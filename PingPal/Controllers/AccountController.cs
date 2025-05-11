using Microsoft.AspNetCore.Mvc;
using PingPal.Database.Context.Factory;
using PingPal.Domain.Dtos.User;
using PingPal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PingPal.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace PingPal.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger, SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(loginModel);
        }
        var result = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password, false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
        }

        return View(loginModel);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home"); 
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel registerModel)
    {
        var conflictedUser = await _userManager.FindByNameAsync(registerModel.Login);
        if (conflictedUser != null)
            ModelState.AddModelError(nameof(registerModel.Login), "Логин уже зарегистрирован");

        if (!ModelState.IsValid)
        {
            return BadRequest(registerModel);
        }
        var user = new User { Id = Guid.NewGuid(), Name = registerModel.Login };

        var result = await _userManager.CreateAsync(user, registerModel.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerModel); 
        }

        await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
