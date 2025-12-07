using EduVision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduVision.Controllers
{
    public class QuestionBankController : Controller
    {
        private readonly EduDbContext _context;

        // Dependency Injection ile veritabanını alıyoruz
        public QuestionBankController(EduDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Veritabanındaki aktif soruları Sınav Türü ve Derse göre grupluyoruz
            // Böylece her "Sınav-Ders" çifti bir Soru Bankası kartı olacak.
            var questionBanks = await _context.Questions
                .Include(q => q.ExamType)
                .Include(q => q.Lesson)
                .Where(q => q.IsActive == true)
                .GroupBy(q => new { q.ExamTypeId, q.ExamType.ExamName, q.LessonId, q.Lesson.LessonName })
                .Select(g => new QuestionBankViewModel
                {
                    ExamTypeId = g.Key.ExamTypeId,
                    LessonId = g.Key.LessonId,
                    ExamName = g.Key.ExamName,
                    LessonName = g.Key.LessonName,
                    QuestionCount = g.Count() // O derste kaç soru var?
                })
                .ToListAsync();

            return View(questionBanks);
        }

        // Detay sayfası (İleride burayı da dolduracağız)
        public async Task<IActionResult> Details(int examTypeId, int lessonId)
        {
            // Veritabanından seçilen derse ve sınav türüne ait soruları çekiyoruz
            var questions = await _context.Questions
                .Include(q => q.ExamType) // Başlıkta yazdırmak için (Örn: TYT)
                .Include(q => q.Lesson)   // Başlıkta yazdırmak için (Örn: Matematik)
                .Where(q => q.ExamTypeId == examTypeId && q.LessonId == lessonId && q.IsActive == true)
                .ToListAsync();

            // Eğer hiç soru yoksa hata vermesin, boş liste dönsün
            if (questions == null || !questions.Any())
            {
                ViewData["Title"] = "Soru Bulunamadı";
                return View(new List<EduVision.Models.Question>());
            }

            // Sayfa başlığı için (Örn: TYT - Matematik Soruları)
            var firstQ = questions.First();
            ViewData["Title"] = $"{firstQ.ExamType.ExamName} - {firstQ.Lesson.LessonName} Soruları";

            return View(questions);
        }
    }

    // View'a veri taşımak için basit bir model
    public class QuestionBankViewModel
    {
        public int ExamTypeId { get; set; }
        public int LessonId { get; set; }
        public string ExamName { get; set; }
        public string LessonName { get; set; }
        public int QuestionCount { get; set; }
    }
}