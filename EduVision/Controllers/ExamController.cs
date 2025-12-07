using EduVision.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EduVision.Controllers
{
    public class ExamController : Controller
    {
        private readonly string _connStr;

        public ExamController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("EduDb");
        }

        // GET: /Exam/Index
        // Burayı istersen basit bir sınav menüsü/Dashboard için kullanırsın
        public IActionResult Index()
        {
            return View();   // Views/Exam/Index.cshtml
        }

        // GET: /Exam/TopicScan?examTypeId=1&lessonId=1
        public IActionResult TopicScan(int examTypeId = 1, int lessonId = 1)
        {
            var vm = new ExamSummaryViewModel();

            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();

                int examId = 0;

                // A) SQL'de sınav oluştur (sp_CreateTopicScanExam)
                using (var cmd = new SqlCommand("Sinav.sp_CreateTopicScanExam", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamTypeId", examTypeId);
                    cmd.Parameters.AddWithValue("@LessonId", lessonId);
                    cmd.Parameters.AddWithValue("@QuestionPerTopic", 1);
                    cmd.Parameters.AddWithValue("@Difficulty", 2);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            examId = (int)reader["ExamId"];   // prosedürün SELECT'inden geliyor
                            vm.ExamId = examId;

                            vm.ExamTitle = reader["Title"].ToString();
                            vm.LevelCode = examTypeId == 1 ? "TYT" : "AYT";
                            vm.CategoryName = "Konu Tarama";
                            vm.QuestionCount = (int)reader["QuestionCount"];
                            vm.DurationMinutes = (int)reader["DurationMin"];
                            vm.Description = reader["Description"].ToString();
                            vm.TargetCorrect = reader["TargetCorrect"] as int?;

                            byte? diff = reader["DifficultyLevel"] as byte?;
                            vm.DifficultyText = diff == 1 ? "Kolay"
                                                 : diff == 2 ? "Orta"
                                                 : diff == 3 ? "Zor"
                                                 : "-";
                        }
                    }
                }

                // B) Bu sınava dahil olan konuları çek (Sinav.ExamTopics + Topics)
                if (examId > 0)
                {
                    using (var cmdTopics = new SqlCommand(@"
                        SELECT t.TopicName
                        FROM Sinav.ExamTopics et
                        JOIN Topics t ON t.TopicId = et.TopicId
                        WHERE et.ExamId = @ExamId
                        ORDER BY t.TopicName;", conn))
                    {
                        cmdTopics.Parameters.AddWithValue("@ExamId", examId);

                        using (var reader = cmdTopics.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                vm.Topics.Add(reader["TopicName"].ToString());
                            }
                        }
                    }
                }

                // C) Sınav geçmişi (vw_ExamHistory'den)
                using (var cmd2 = new SqlCommand(@"
                    SELECT TOP 10 *
                    FROM Sinav.vw_ExamHistory
                    ORDER BY ExamDate DESC;", conn))
                {
                    using (var reader = cmd2.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vm.History.Add(new ExamHistoryItem
                            {
                                ExamResultId = (int)reader["ExamResultId"],
                                ExamName = reader["ExamName"].ToString(),
                                ExamDate = (DateTime)reader["ExamDate"],
                                CategoryName = reader["ExamTypeName"].ToString(),
                                Net = (decimal)reader["Net"],
                                DurationMinutes = System.Convert.ToDouble(reader["DurationMin"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }

            // Views/Exam/TopicScan.cshtml
            return View(vm);
        }
    }
}
