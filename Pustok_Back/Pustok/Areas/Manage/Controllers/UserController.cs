using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels.AccountViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize( Roles = "Admin" ) ]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<AppUser> appUsers = await _userManager.Users.ToListAsync();

            List<AppUserVM> appUserVMs = new List<AppUserVM>();

            foreach (AppUser appUser in appUsers)
            {
                AppUserVM appUserVM = new AppUserVM
                {
                    Id = appUser.Id,
                    Name = appUser.Name,
                    SurName = appUser.SurName,
                    Age = appUser.Age,
                    Email = appUser.Email,
                    FatherName = appUser.FatherName,
                    Role = (await _userManager.GetRolesAsync(appUser))[0],
                    UserName = appUser.UserName,
                    IsDeleted = appUser.IsDeleted
                };

                appUserVMs.Add(appUserVM);
            }

            return View(appUserVMs);
        }

        public async Task<IActionResult> ChangeStatus(string Id, bool status)
        {
            if (Id == null) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(Id);

            if (appUser == null) return NotFound();

            appUser.IsDeleted = status;
            await _userManager.UpdateAsync(appUser);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangeRole(string Id)
        {
            if (Id == null) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(Id);

            if (appUser == null) return NotFound();

            AppUserVM appUserVM = new AppUserVM
            {
                Id = appUser.Id,
                Name = appUser.Name,
                SurName = appUser.SurName,
                Age = appUser.Age,
                Email = appUser.Email,
                FatherName = appUser.FatherName,
                Role = (await _userManager.GetRolesAsync(appUser))[0],
                UserName = appUser.UserName,
                IsDeleted = appUser.IsDeleted,
                Roles = new List<string> { "Admin","Member"}
            };

            return View(appUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string Id, string Roles)
        {
            if (Id == null) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(Id);

            if (appUser == null) return NotFound();

            string oldRole = (await _userManager.GetRolesAsync(appUser))[0];

            await _userManager.RemoveFromRoleAsync(appUser, oldRole);

            await _userManager.AddToRoleAsync(appUser, Roles);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ChangePassword(string Id)
        {
            if (Id == null) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(Id);

            if (appUser == null) return NotFound();

            AppUserVM appUserVM = new AppUserVM
            {
                Id = appUser.Id,
                Name = appUser.Name,
                SurName = appUser.SurName,
                Age = appUser.Age,
                Email = appUser.Email,
                FatherName = appUser.FatherName,
                Role = (await _userManager.GetRolesAsync(appUser))[0],
                UserName = appUser.UserName,
                IsDeleted = appUser.IsDeleted,
                Roles = new List<string> { "Admin", "Member" }
            };

            return View(appUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string Id, string Password)
        {
            if (Id == null) return NotFound();

            AppUser appUser = await _userManager.FindByIdAsync(Id);

            if (appUser == null) return NotFound();

            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

            await _userManager.ResetPasswordAsync(appUser, token, Password);

            return RedirectToAction("Index");
        }
    }
}
