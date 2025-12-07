using System;
using System.Collections.Generic;

namespace EduVision.Models;

public partial class DailyStat
{
    public int StatId { get; set; }

    public DateOnly? ReportDate { get; set; }

    public int? TotalQuestions { get; set; }

    public int? ActiveQuestions { get; set; }
}
