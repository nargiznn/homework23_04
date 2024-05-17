using System;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Models;
using Pustok.ViewModels;
using System.Security.Claims;
using Pustok.Data;
using Microsoft.EntityFrameworkCore;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid) return View();

            if (_userManager.Users.Any(x => x.NormalizedEmail == registerVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View();
            }

            AppUser user = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FullName = registerVM.FullName,
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    if (err.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    else ModelState.AddModelError("", err.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, "member");


            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string? returnUrl)
        {
            if ((!ModelState.IsValid)) return View();

            AppUser? user = await _userManager.FindByEmailAsync(loginVM.Email);

            if (user == null || !await _userManager.IsInRoleAsync(user, "member"))
            {
                ModelState.AddModelError("", "Email or Pasword incorrect!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "You are locked out for 5 minutes!");
                return View();
            }
            else if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Pasword incorrect!");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");

        }

        [Authorize(Roles = "member")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [Authorize(Roles = "member")]
        public async Task<IActionResult> Profile(string tab = "dashboard")
        {
            AppUser? user = await _userManager.GetUserAsync(User);

            if (user == null)
                return RedirectToAction("login", "account");

            ProfileViewModel profileVM = new ProfileViewModel
            {
                ProfileEditVM = new ProfileEdirViewModel
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName
                },
                Orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(oi => oi.Book).OrderByDescending(x => x.CreatedAt)
                .Where(x => x.AppUserId == user.Id).ToList()
            };

            ViewBag.Tab = tab;

            return View(profileVM);
        }

        [Authorize(Roles = "member")]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileEdirViewModel profileEditVM, string tab = "profile")
        {
            ViewBag.Tab = tab;
            ProfileViewModel profileVM = new ProfileViewModel();
            profileVM.ProfileEditVM = profileEditVM;

            if (!ModelState.IsValid) return View(profileVM);

            AppUser? user = await _userManager.GetUserAsync(User);

            if (user == null) return RedirectToAction("login", "account");

            user.UserName = profileEditVM.UserName;
            user.Email = profileEditVM.Email;
            user.FullName = profileEditVM.FullName;

            if (_userManager.Users.Any(x => x.Id != User.FindFirstValue(ClaimTypes.NameIdentifier) && x.NormalizedEmail == profileEditVM.Email.ToUpper()))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(profileVM);
            }

            if (profileEditVM.NewPassword != null)
            {
                var passwordResult = await _userManager.ChangePasswordAsync(user, profileEditVM.CurrentPassword, profileEditVM.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var err in passwordResult.Errors)
                        ModelState.AddModelError("", err.Description);

                    return View(profileVM);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    if (err.Code == "DuplicateUserName")
                        ModelState.AddModelError("UserName", "UserName is already taken");
                    else ModelState.AddModelError("", err.Description);
                }
                return View(profileVM);
            }
            await _signInManager.SignInAsync(user, false);

            return View(profileVM);
        }


    }
}