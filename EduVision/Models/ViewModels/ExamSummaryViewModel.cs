using System;
using System.Collections.Generic;

namespace EduVision.Models.ViewModels
{
    public class ExamSummaryViewModel
    {
        public int ExamId { get; set; }               // yeni: sınav id
        public string ExamTitle { get; set; }         // "Matematik Konu Tarama - TYT"
        public string LevelCode { get; set; }         // "TYT" / "AYT"
        public string CategoryName { get; set; }      // "Konu Tarama"
        public int QuestionCount { get; set; }        // 25
        public string DifficultyText { get; set; }    // "Orta"
        public int DurationMinutes { get; set; }      // 25

        public string Description { get; set; }       // sağ karttaki açıklama
        public int? TargetCorrect { get; set; }       // 18+

        // Bu sınava dahil olan konular
        public List<string> Topics { get; set; } = new();

        // Alt taraftaki sınav geçmişi tablosu
        public List<ExamHistoryItem> History { get; set; } = new();
    }

    public class ExamHistoryItem
    {
        public int ExamResultId { get; set; }
        public string ExamName { get; set; }
        public DateTime ExamDate { get; set; }
        public string CategoryName { get; set; }   // "Konu Tarama", "Deneme"
        public decimal Net { get; set; }
        public double DurationMinutes { get; set; }
        public string Status { get; set; }
    }
}
