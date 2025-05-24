using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PingPal;
using PingPal.Domain.Dtos;
using PingPal.Domain.Dtos.User;
using PingPal.Domain.Entities;
using PingPal.Exceptions;
using PingPal.Extensions;

namespace AspNetExample.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/account")]
[Produces(MediaTypeNames.Application.Json)]
public class AccountApiController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AccountApiController(
        UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<TokenDto> Login(
        [FromBody] LoginDto login)
    {
        if (login.Login.IsNullOrEmpty())
            ModelState.AddModelError(nameof(login.Login), "Логин обязателен для заполнения");

        if (login.Password.IsNullOrEmpty())
            ModelState.AddModelError(nameof(login.Password), "Пароль обязателен для заполнения");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var user = await _userManager.FindByNameAsync(login.Login);
        if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
            throw new UnauthorizedException("Некорректные логин и(или) пароль");

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            }
            .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var symmetricSecurityKey = Constants.GetJwtSymmetricSecurityKey();
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            Constants.JwtIssuer,
            Constants.JwtAudience,
            claims,
            expires: DateTime.UtcNow.Add(Constants.JwtLifetime),
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return new TokenDto { Token = token };
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task Register(
        [FromBody] RegisterDto register)
    {
        if (register.Login.IsNullOrEmpty())
            ModelState.AddModelError(nameof(register.Login), "Логин обязателен для заполнения");

        if (register.Password.IsNullOrEmpty())
            ModelState.AddModelError(nameof(register.Password), "Пароль обязателен для заполнения");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        if (register.Password.Length < 6)
            ModelState.AddModelError(nameof(register.Password), "Минимальная длина пароля 6 символов");

        var conflictedUser = await _userManager.FindByNameAsync(register.Login);
        if (conflictedUser != null)
            ModelState.AddModelError(nameof(register.Login), "Логин уже зарегистрирован");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var user = new User { Id = Guid.NewGuid(), Name = register.Login };
        var result = await _userManager.CreateAsync(user, register.Password);
        if (!result.Succeeded)
            throw new InternalServerErrorException(ModelState.JoinErrors());
    }

    [HttpPost("changePassword")]
    public async Task ChangePassword(
        [FromBody] ChangePasswordDto changePassword)
    {
        if (changePassword.OldPassword.IsNullOrEmpty())
            ModelState.AddModelError(nameof(changePassword.OldPassword), "Старый пароль обязателен для заполнения");

        if (changePassword.NewPassword.IsNullOrEmpty())
            ModelState.AddModelError(nameof(changePassword.NewPassword), "Новый пароль обязателен для заполнения");
        else if (changePassword.NewPassword.Length < 6)
            ModelState.AddModelError(nameof(changePassword.NewPassword), "Минимальная длина нового пароля 6 символов");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var user = await _userManager.GetUserAsync(HttpContext.User)
            ?? throw new InvalidOperationException("User is null");

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
        if (!changePasswordResult.Succeeded)
            throw new BadRequestException("Старый пароль некорректен");
    }
}