using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingPal.Domain.Entities;
using PingPal.Models.Account;

namespace PingPal.Controllers
{
    [AllowAnonymous]
	[Route("Account")]
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
		public IActionResult Login(string returnUrl = null)
		{
			var model = new LoginModel { ReturnUrl = returnUrl };
			return View(model);
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
                return RedirectToAction("Index", "Users");
            }
            else ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");

            return RedirectToAction("Index", "Users");
        }	
		[HttpGet("register")]
		public IActionResult Register(string returnUrl = null)
		{
			var model = new RegisterModel { ReturnUrl = returnUrl };
			return View(model);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromForm] RegisterModel registerModel)
		{
			if (!ModelState.IsValid)
			{
				return View(registerModel);
			}

			var conflictedUser = await _userManager.FindByNameAsync(registerModel.Login);
			if (conflictedUser != null)
			{
				ModelState.AddModelError(nameof(registerModel.Login), "Логин уже зарегистрирован");
				return View(registerModel);
			}

			var user = new User
			{
				Id = Guid.NewGuid(),
				Name = registerModel.Login
			};

			var result = await _userManager.CreateAsync(user, registerModel.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View(registerModel);
			}
			
			await _signInManager.SignInAsync(user, isPersistent: false);
			
			return RedirectToAction("Index", "Users");
		}

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
