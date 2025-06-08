using PingPal.Common.Extensions;
using PingPal.Service.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingPal.Domain.Entities;
using PingPal.Models.Account;

namespace PingPal.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContextSignInManager _applicationContextSignInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApplicationContextUserManager applicationContextUserManager,
        ApplicationContextSignInManager applicationContextSignInManager,
        ILogger<AccountController> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _applicationContextSignInManager = applicationContextSignInManager;
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Login(
        [FromQuery] string? returnUrl = null)
    {
        return View(new LoginModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _applicationContextSignInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(model.Login), "Некорректные логин и(или) пароль");
            return View(model);
        }

        if (model.ReturnUrl.IsSignificant())
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Users");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(
        [FromQuery] string? returnUrl = null)
    {
        return View(new RegisterModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        [FromForm] RegisterModel model)
    {
        var conflictedUser = await _applicationContextUserManager.FindByNameAsync(model.Login);
        if (conflictedUser != null)
            ModelState.AddModelError(nameof(model.Login), "Логин уже зарегистрирован");

        if (!ModelState.IsValid)
            return View(model);

        var user = new User { Id = Guid.NewGuid(), Name = model.Login };
        var result = await _applicationContextUserManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            result.Errors.ForEach(error => ModelState.AddModelError(nameof(model.Login), error.Description));
            return View(model);
        }

        await _applicationContextSignInManager.SignInAsync(user, false);

        if (model.ReturnUrl.IsSignificant())
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _applicationContextSignInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}