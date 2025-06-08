using PingPal.Common.Extensions;
using PingPal.Extensions.Models;
using PingPal.Service.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingPal.Domain;
using PingPal.Domain.Entities;
using PingPal.Models.Users;

namespace PingPal.Controllers;

[Authorize(Roles = RoleTokens.AdminRole)]
public class UsersController : Controller
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        ApplicationContextUserManager applicationContextUserManager,
        ILogger<UsersController> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        [FromQuery] UsersIndexModel? model,
        CancellationToken cancellationToken)
    {
        var usersQuery = _applicationContextUserManager.Users
            .Include(user => user.UserRoles)
            .ThenInclude(userRole => userRole.Role)
            .AsNoTracking();

        var searchString = model?.SearchString;

        if (searchString.IsSignificant())
            usersQuery = usersQuery.Where(user =>
                EF.Functions.Like(user.Id.ToString(), $"%{searchString}%") ||
                EF.Functions.Like(user.Name, $"%{searchString}%"));

        usersQuery = model?.SortBy switch
        {
            nameof(UserModel.Id) => usersQuery.OrderBy(user => user.Id),
            nameof(UserModel.Id) + Constants.DescSuffix => usersQuery.OrderByDescending(user => user.Id),
            nameof(UserModel.Login) => usersQuery.OrderBy(user => user.Name),
            nameof(UserModel.Login) + Constants.DescSuffix => usersQuery.OrderByDescending(user => user.Name),
            _ => usersQuery.OrderBy(user => user.Id)
        };

        var page = Math.Max(Constants.FirstPage, model?.Page ?? Constants.FirstPage);
        var totalCount = await usersQuery.CountAsync(cancellationToken);
        var users = await usersQuery
            .Skip((page - Constants.FirstPage) * Constants.PageSize)
            .Take(Constants.PageSize)
            .ToArrayAsync(cancellationToken);
        var userModels = await users
            .ToModelsAsync(_applicationContextUserManager)
            .ToArrayAsync(cancellationToken);

        return View(new UsersIndexModel
        {
            Users = userModels,
            SortBy = model?.SortBy,
            Page = page,
            TotalCount = totalCount
        });
    }

    public IActionResult Create()
    {
        return View(new UserModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromForm] UserModel model)
    {
        if (model.NewPassword.IsNullOrEmpty())
            ModelState.AddModelError(nameof(model.NewPassword), "Не указан пароль");

        var conflictedUser = await _applicationContextUserManager.FindByNameAsync(model.Login);
        if (conflictedUser != null)
            ModelState.AddModelError(nameof(model.Login), "Логин уже зарегистрирован");

        if (!ModelState.IsValid)
            return View(model);

        var user = new User { Id = Guid.NewGuid(), Name = model.Login };
        await _applicationContextUserManager.CreateAsync(user, model.NewPassword!);

        if (model.HasAdminRole)
            await _applicationContextUserManager.AddToRoleAsync(user, RoleTokens.AdminRole);
        if (model.HasSwaggerRole)
            await _applicationContextUserManager.AddToRoleAsync(user, RoleTokens.SwaggerRole);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("[controller]/[action]/{id:guid}")]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid id)
    {
        var user = await _applicationContextUserManager.FindByIdAndLoadRolesAsync(id.ToString());
        if (user == null)
            return NotFound();

        return View(await user.ToModelAsync(_applicationContextUserManager));
    }

    [HttpPost("[controller]/[action]/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid id,
        [FromForm] UserModel model,
        CancellationToken cancellationToken)
    {
		if (model.NewPassword.IsSignificant())
		{
			var validationTasks = _applicationContextUserManager.PasswordValidators
				.Select(passwordValidator => passwordValidator.ValidateAsync(_applicationContextUserManager, null!, model.NewPassword))
				.ToArray();
			var results = await Task.WhenAll(validationTasks);
			if (results.Any(result => !result.Succeeded))
			{
				foreach (var result in results.Where(r => !r.Succeeded))
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(nameof(model.NewPassword), error.Description);
					}
				}
			}
		}

        if (!ModelState.IsValid)
            return View(model);

        var user = await _applicationContextUserManager./*FindByIdAndLoadRolesAsync*/FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound();

        if (user.Name != model.Login)
        {
            user.Name = model.Login;
            await _applicationContextUserManager.UpdateAsync(user);
        }

        if (model.NewPassword.IsSignificant())
        {
            user.PasswordHash = _applicationContextUserManager.PasswordHasher.HashPassword(user, model.NewPassword);
            await _applicationContextUserManager.UpdateAsync(user);
        }

        //var roles = await _applicationContextUserManager.GetRolesAsync(user);

        //if (roles.Contains(RoleTokens.AdminRole) != model.HasAdminRole)
        //{
        //    if (model.HasAdminRole)
        //        await _applicationContextUserManager.AddToRoleAsync(user, RoleTokens.AdminRole);
        //    else
        //        await _applicationContextUserManager.RemoveFromRoleAsync(user, RoleTokens.AdminRole);
        //}

        //if (roles.Contains(RoleTokens.SwaggerRole) != model.HasSwaggerRole)
        //{
        //    if (model.HasSwaggerRole)
        //        await _applicationContextUserManager.AddToRoleAsync(user, RoleTokens.SwaggerRole);
        //    else
        //        await _applicationContextUserManager.RemoveFromRoleAsync(user, RoleTokens.SwaggerRole);
        //}

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("[controller]/[action]/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(id.ToString());
        if (user == null)
            return NotFound();

        await _applicationContextUserManager.DeleteAsync(user);

        return RedirectToAction(nameof(Index));
    }
}