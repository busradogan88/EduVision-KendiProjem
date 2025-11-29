using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            // Welcome sayfanı döndür
            return View("Welcome");
        }
        public IActionResult Welcome()
        {
            return View(); // Views/Account/Welcome.cshtml
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
