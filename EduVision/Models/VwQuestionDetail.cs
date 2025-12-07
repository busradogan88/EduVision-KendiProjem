using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class VwQuestionDetail
{
    public int QuestionId { get; set; }

    public int QuestionBankId { get; set; }

    public string BankName { get; set; } = null!;

    public string? Publisher { get; set; }

    public string? CoverImageUrl { get; set; }

    public string ExamName { get; set; } = null!;

    public string LessonName { get; set; } = null!;

    public string QuestionText { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public string? DifficultyText { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }
}
