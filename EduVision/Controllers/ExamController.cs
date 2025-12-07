using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduVision.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
