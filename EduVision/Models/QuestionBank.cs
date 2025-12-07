using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class QuestionBank
{
    public int QuestionBankId { get; set; }

    public int ExamTypeId { get; set; }

    public int LessonId { get; set; }

    public string BankName { get; set; } = null!;

    public string? Publisher { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public virtual ExamType ExamType { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
