using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public string LessonName { get; set; } = null!;

    public virtual ICollection<QuestionBank> QuestionBanks { get; set; } = new List<QuestionBank>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
