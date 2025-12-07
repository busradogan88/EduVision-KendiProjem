using System.Security.Claims;
using EduVision.Data;
using EduVision.Helpers;
using EduVision.Models;
using EduVision.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _repo;

        public AccountController(AccountRepository repo)
        {
            _repo = repo;
        }

        // ✅ İLK AÇILIŞ SAYFASI (WELCOME)
        [HttpGet]
        public IActionResult Welcome()
        {
            return View(); // Views/Account/Welcome.cshtml
        }

        // ✅ KAYIT OL - GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ✅ KAYIT OL - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_repo.EmailExists(model.Email))
            {
                ModelState.AddModelError("", "Bu email ile zaten kullanıcı var.");
                return View(model);
            }

            PasswordHelper.CreatePasswordHash(
                model.Password,
                out var hash,
                out var salt);

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            user.Id = _repo.InsertUser(user);

            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        // ✅ GİRİŞ - GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ✅ GİRİŞ - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _repo.GetUserByEmail(model.Email);

            if (user == null ||
                !PasswordHelper.VerifyPassword(
                    model.Password,
                    user.PasswordHash,
                    user.PasswordSalt))
            {
                ModelState.AddModelError("", "Email veya şifre hatalı.");
                return View(model);
            }

            await SignInUser(user);

            return RedirectToAction("Index", "Home");
        }

        // ✅ ÇIKIŞ
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // ✅ COOKIE OLUŞTURMA
        private async Task SignInUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }
    }
}
