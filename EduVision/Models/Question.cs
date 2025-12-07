using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuestionBankId { get; set; }

    public int ExamTypeId { get; set; }

    public int LessonId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    public string OptionE { get; set; } = null!;

    public string CorrectAnswer { get; set; } = null!;

    public byte? DifficultyLevel { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ExamType ExamType { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual QuestionBank QuestionBank { get; set; } = null!;
}
