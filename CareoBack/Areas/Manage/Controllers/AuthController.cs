using CareoBack.Areas.Manage.ViewModels;
using CareoBack.DAL;
using CareoBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareoBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AuthController : Controller
    {
        private Context Context { get; }
        private UserManager<AppUser> UserManager { get; }
        private SignInManager<AppUser> SignInManager { get; }
        public AuthController(Context context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInVm signInVm)
        {
            AppUser user = await UserManager.FindByEmailAsync(signInVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View(signInVm);
            }
            var result = await SignInManager.PasswordSignInAsync(user, signInVm.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password Incorrect!");
                return View(signInVm);
            }
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if (!ModelState.IsValid) return View(registerVm);
            AppUser user = new AppUser
            {
                UserName = registerVm.Username,
                Email = registerVm.Email,
            };
            IdentityResult result = await UserManager.CreateAsync(user, registerVm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    return View();
                }
            }
            await SignInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> SignOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}
