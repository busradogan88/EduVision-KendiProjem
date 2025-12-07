using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class QuestionLog
{
    public int LogId { get; set; }

    public int? QuestionId { get; set; }

    public string? ActionType { get; set; }

    public DateTime? LogDate { get; set; }
}
