namespace EduVision.Models.ViewModels
{
    public class QuestionBankDetailsViewModel
    {
        public int ExamTypeId { get; set; }
        public int LessonId { get; set; }

        public string ExamName { get; set; }
        public string LessonName { get; set; }
        public string BankName { get; set; }

        public int TotalQuestions { get; set; }

        // Sayfalama
        public int CurrentPage { get; set; }   // 0 = Kapak, 1..N = soru sayfaları
        public int TotalPages { get; set; }
        public bool ShowCover { get; set; }
        public List<string> Topics { get; set; } = new();


        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
