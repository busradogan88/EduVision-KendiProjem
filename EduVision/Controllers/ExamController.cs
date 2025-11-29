using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    public class ExamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
