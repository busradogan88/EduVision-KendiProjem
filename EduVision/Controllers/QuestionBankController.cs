using EduVision.Models;
using EduVision.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduVision.Controllers
{
    public class QuestionBankController : Controller
    {
        private readonly EduDbContext _context;

        public QuestionBankController(EduDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var banks = await _context.QuestionBanks
                .Include(qb => qb.ExamType)
                .Include(qb => qb.Lesson)
                .Include(qb => qb.Questions)
                .Where(qb => qb.IsActive == true)
                .Select(qb => new QuestionBankViewModel
                {
                    QuestionBankId = qb.QuestionBankId,
                    ExamTypeId = qb.ExamTypeId,
                    LessonId = qb.LessonId,
                    BankName = qb.BankName,
                    ExamName = qb.ExamType.ExamName,
                    LessonName = qb.Lesson.LessonName,
                    Publisher = qb.Publisher,
                    ImageUrl = qb.CoverImageUrl,
                    QuestionCount = qb.Questions.Count(q => q.IsActive == true)
                })
                .ToListAsync();

            return View(banks);
        }

        // 🔹 examTypeId + lessonId ile gelen bankanın kapak + sayfalı soruları
        public async Task<IActionResult> Details(int examTypeId, int lessonId, int page = 0)
        {
            const int pageSize = 10;

            // Bu bankaya ait aktif sorular
            var baseQuery = _context.Questions
                .Include(q => q.ExamType)
                .Include(q => q.Lesson)
                .Where(q => q.ExamTypeId == examTypeId
                            && q.LessonId == lessonId
                            && q.IsActive == true);

            var totalQuestions = await baseQuery.CountAsync();

            // ✅ KONULARI ÇEK (BURASI EKLENDİ)
            var topics = await _context.Topics
                .Where(t => t.ExamTypeId == examTypeId
                         && t.LessonId == lessonId
                         && t.IsActive)
                .Select(t => t.TopicName)
                .ToListAsync();

            // Hiç soru yoksa
            if (totalQuestions == 0)
            {
                var emptyModel = new QuestionBankDetailsViewModel
                {
                    ExamTypeId = examTypeId,
                    LessonId = lessonId,
                    BankName = "Soru Bulunamadı",
                    TotalQuestions = 0,
                    CurrentPage = 0,
                    TotalPages = 0,
                    ShowCover = true,
                    Topics = topics   // ✅ EKLENDİ
                };

                ViewData["Title"] = emptyModel.BankName;
                return View(emptyModel);
            }

            // Bir soru çekip sınav/ders isimlerini ve varsayılan başlığı alalım
            var firstQuestion = await baseQuery
                .OrderBy(q => q.QuestionId)
                .FirstAsync();

            // QuestionBanks tablosundan banka bilgisi (varsa)
            var bank = await _context.QuestionBanks
                .Include(qb => qb.ExamType)
                .Include(qb => qb.Lesson)
                .FirstOrDefaultAsync(qb =>
                    qb.ExamTypeId == examTypeId &&
                    qb.LessonId == lessonId &&
                    qb.IsActive == true);

            string bankName = bank?.BankName
                ?? $"{firstQuestion.ExamType.ExamName} {firstQuestion.Lesson.LessonName} Soru Bankası";

            string examName = bank?.ExamType?.ExamName ?? firstQuestion.ExamType.ExamName;
            string lessonName = bank?.Lesson?.LessonName ?? firstQuestion.Lesson.LessonName;

            int totalPages = (int)Math.Ceiling(totalQuestions / (double)pageSize);

            // 0. sayfa = KAPAK
            if (page == 0)
            {
                var coverModel = new QuestionBankDetailsViewModel
                {
                    ExamTypeId = examTypeId,
                    LessonId = lessonId,
                    ExamName = examName,
                    LessonName = lessonName,
                    BankName = bankName,
                    TotalQuestions = totalQuestions,
                    CurrentPage = 0,
                    TotalPages = totalPages,
                    ShowCover = true,
                    Topics = topics   // ✅ EKLENDİ
                };

                ViewData["Title"] = bankName;
                return View(coverModel);
            }

            // Sayfa aralığını zorla
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var questionsPage = await baseQuery
                .OrderBy(q => q.QuestionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new QuestionBankDetailsViewModel
            {
                ExamTypeId = examTypeId,
                LessonId = lessonId,
                ExamName = examName,
                LessonName = lessonName,
                BankName = bankName,
                TotalQuestions = totalQuestions,
                CurrentPage = page,
                TotalPages = totalPages,
                ShowCover = false,
                Questions = questionsPage,
                Topics = topics   // ✅ EKLENDİ
            };

            ViewData["Title"] = $"{bankName} - Sayfa {page}";
            return View(model);
        }

    }

    public class QuestionBankViewModel
    {
        public int QuestionBankId { get; set; }
        public int ExamTypeId { get; set; }
        public int LessonId { get; set; }

        public string BankName { get; set; }
        public string ExamName { get; set; }
        public string LessonName { get; set; }
        public string Publisher { get; set; }
        public string ImageUrl { get; set; }

        public int QuestionCount { get; set; }
    }
}
