using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
