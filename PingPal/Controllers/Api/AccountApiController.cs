using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using PingPal.Extensions;
using PingPal.Service.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PingPal.Domain.Dtos;
using PingPal.Domain.Entities;
using PingPal.Exceptions.Api;

namespace PingPal.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/account")]
[Produces(MediaTypeNames.Application.Json)]
public class AccountApiController : ControllerBase
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;

    public AccountApiController(
        ApplicationContextUserManager applicationContextUserManager)
    {
        _applicationContextUserManager = applicationContextUserManager;
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

        var user = await _applicationContextUserManager./*FindByNameAndLoadRolesAsync*/FindByIdAsync(login.Login);
        if (user == null || !await _applicationContextUserManager.CheckPasswordAsync(user, login.Password))
            throw new UnauthorizedException("Некорректные логин и(или) пароль");

        var token = await CreateJwtToken(user);
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

        var conflictedUser = await _applicationContextUserManager.FindByNameAsync(register.Login);
        if (conflictedUser != null)
            ModelState.AddModelError(nameof(register.Login), "Логин уже зарегистрирован");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var user = new User { Id = Guid.NewGuid(), Name = register.Login };
        var result = await _applicationContextUserManager.CreateAsync(user, register.Password);
        if (!result.Succeeded)
            throw new InternalServerErrorException(result.Errors.JoinErrors());
    }

    [HttpPost("refresh")]
    public async Task<TokenDto> RefreshToken()
    {
        var user = await _applicationContextUserManager.GetUserAndLoadRolesAsync(HttpContext.User)
            ?? throw new InvalidOperationException("User is null");

        var newToken = await CreateJwtToken(user);
        return new TokenDto { Token = newToken };
    }

    [HttpPost("change-password")]
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

        var user = await _applicationContextUserManager.GetUserAsync(HttpContext.User)
            ?? throw new InvalidOperationException("User is null");

        var changePasswordResult = await _applicationContextUserManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
        if (!changePasswordResult.Succeeded)
            throw new BadRequestException("Старый пароль некорректен");
    }

    private async Task<string> CreateJwtToken(User user)
    {
        var roles = await _applicationContextUserManager.GetRolesAsync(user);
        var claims = roles.Select(role => new Claim(ClaimTypes.Role, role))
            .Prepend(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()))
            .Prepend(new Claim(ClaimTypes.Name, user.Name));

        var symmetricSecurityKey = Constants.GetJwtSymmetricSecurityKey();
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var utcNow = DateTime.UtcNow;
        var jwtSecurityToken = new JwtSecurityToken(
            Constants.JwtIssuer,
            Constants.JwtAudience,
            claims,
            utcNow,
            utcNow.Add(Constants.JwtLifetime),
            signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }
}