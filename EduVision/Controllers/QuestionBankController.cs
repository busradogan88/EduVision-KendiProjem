using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    public class QuestionBankController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {

            return View();
        }
    }
}
