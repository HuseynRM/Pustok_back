using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Asn1.Ocsp;
using Pustok.Models;
using Pustok.ViewModels.AccountViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;

        public AccountController(IWebHostEnvironment env, UserManager<AppUser> manager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _manager = manager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _env = env;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return NotFound();

            AppUser appUser = await _manager.FindByEmailAsync(loginVM.Email);

            if(appUser == null)
            {
                ModelState.AddModelError("", "Email Ve Ya Shifre Yanlisdir");
                return View(loginVM);
            }

            if (appUser.EmailConfirmed == false)
            {
                return RedirectToAction("Index", "Home");
            }

            Microsoft.AspNetCore.Identity.SignInResult signinResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, true, true);

            if (signinResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Email Bloklanib");
                return View(loginVM);
            }

            if (!signinResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Ve Ya Shifre Yanlisdir");
                return View(loginVM);
            }
            
            if ((await _manager.GetRolesAsync(appUser))[0] == "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Age = registerVM.Age,
                Email = registerVM.Email,           
                UserName = registerVM.UserName
            };

            IdentityResult identityResult = await _manager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _manager.AddToRoleAsync(appUser, "Member");
            await _signInManager.SignInAsync(appUser, true);

            // create email message

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("huseynrm@mail.ru"));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Tesdiqleme E-maili";
            message.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };
            string emailbody = string.Empty;

            using (StreamReader streamReader = new StreamReader(Path.Combine(_env.WebRootPath,"Templates", "Emailconfirm.html")))
            {
                emailbody = streamReader.ReadToEnd();
            }

            string emailconfirmtoken = await _manager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("Confirmemail","accaunt",new {id =appUser.Id, token= emailconfirmtoken },Request.Scheme);

            emailbody = emailbody.Replace("{{Fullname}}", $"{ appUser.Name} {appUser.SurName }").Replace("{{url}}", $"{url}");

            message.Body = new TextPart(TextFormat.Html) { Text = emailbody };

                // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.mail.ru", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("huseynrm@mail.ru", "husu234567");
            smtp.Send(message);
            smtp.Disconnect(true);

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Confirmemail(string id, string token)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }
            AppUser appUser = await _manager.FindByIdAsync(id);
            if (appUser== null)
            {
                return NotFound();
            }
            IdentityResult identityResult = await _manager.ConfirmEmailAsync(appUser, token);
            if(!identityResult.Succeeded)
            {
                return NotFound(); 
            }
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByEmailAsync(email);

            if (appUser == null)
            {
                return NotFound();
            }
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("huseynrm@mail.ru"));
            message.To.Add(MailboxAddress.Parse(appUser.Email));
            message.Subject = "Tesdiqleme E-maili";
            message.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };
            string emailbody = string.Empty;

            using (StreamReader streamReader = new StreamReader(Path.Combine(_env.WebRootPath, "Templates", "Emailconfirm.html")))
            {
                emailbody = streamReader.ReadToEnd();
            }

            string passwordconfirmtoken = await _manager.GeneratePasswordResetTokenAsync(appUser);
            string url = Url.Action("changepassword", "accaunt", new { id = appUser.Id, token = passwordconfirmtoken }, Request.Scheme);

            emailbody = emailbody.Replace("{{Fullname}}", $"{ appUser.Name} {appUser.SurName }").Replace("{{url}}", $"{url}");

            message.Body = new TextPart(TextFormat.Html) { Text = emailbody };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.mail.ru", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("huseynrm@mail.ru", "husu234567");
            smtp.Send(message);
            smtp.Disconnect(true);
            return View();
        }
        
        public async Task<IActionResult> Changepassword(string id,string token)
        {
            if (string.IsNullOrWhiteSpace(id)|| string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            ResetpasswordVM resetpasswordVM = new ResetpasswordVM
            {
                Id = id,
                Token = token,
            };

            return View(resetpasswordVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Changepassword(ResetpasswordVM resetpasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrWhiteSpace(resetpasswordVM.Id) || string.IsNullOrWhiteSpace(resetpasswordVM.Token))
            {
                return NotFound();
            }

            AppUser appUser = await _manager.FindByIdAsync(resetpasswordVM.Id);

            if (appUser == null)
            {
                return NotFound();
            }

             IdentityResult identityResult = await _manager.ResetPasswordAsync(appUser, resetpasswordVM.Token, resetpasswordVM.Password);
            
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetpasswordVM);
            }

            return RedirectToAction("Login");
        }
        #region Add Role
        //public async Task<IActionResult> AddRole()
        //{
        //    if (!await _roleManager.RoleExistsAsync("Admin"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    }
        //    if (!await _roleManager.RoleExistsAsync("Member"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });
        //    }
        //    if (!await _roleManager.RoleExistsAsync("User"))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        //    }

        //    return Content("Role Yarandi");
        //}
        #endregion
    }
}
