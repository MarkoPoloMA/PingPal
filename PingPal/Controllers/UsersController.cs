using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PingPal.Domain.Entities;
using PingPal.Models.Users;

namespace PingPal.Controllers
{
	public class UsersController : Controller
	{
		private readonly UserManager<User> _userManager;
		public UsersController(UserManager<User> userManager)
		{
			_userManager = userManager;
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View(new UserModel());
		}
		
		[HttpPost]
		public async Task<IActionResult> Create(UserModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model); 
			}

			var user = new User
			{
				Name = model.Name
			};
			
			var result = await _userManager.CreateAsync(user, model.NewPassword);

			if (result.Succeeded)
			{
				if (model.HasAdminRole)
				{
					await _userManager.AddToRoleAsync(user, "Admin");
				}

				return RedirectToAction(nameof(Index)); 
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model); 
		}
			public IActionResult Index()
			{
				var users = _userManager.Users.ToList(); 
				return View(users);
			}
        [HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			var user = await _userManager.FindByIdAsync(id.ToString());
			if (user == null)
			{
				return NotFound();
			}

			var model = new UserModel
			{
				Id = user.Id,
				Name = user.Name,
				HasAdminRole = await _userManager.IsInRoleAsync(user, "Admin"),
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(UserModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByIdAsync(model.Id.ToString());
			if (user == null)
			{
				return NotFound();
			}

			user.Name = model.Name;

			if (!string.IsNullOrEmpty(model.NewPassword))
			{
				var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
				if (!passwordResult.Succeeded)
				{
					foreach (var error in passwordResult.Errors)
						ModelState.AddModelError(string.Empty, error.Description);

					return View(model);
				}
			}

			if (model.HasAdminRole)
			{
				if (!await _userManager.IsInRoleAsync(user, "Admin"))
					await _userManager.AddToRoleAsync(user, "Admin");
			}
			else
			{
				if (await _userManager.IsInRoleAsync(user, "Admin"))
					await _userManager.RemoveFromRoleAsync(user, "Admin");
			}

			await _userManager.UpdateAsync(user);
			return RedirectToAction(nameof(Index));
		}
	}
}
