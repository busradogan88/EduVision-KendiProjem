namespace EduVision.Models
{
    public class Topic
    {
        public int TopicId { get; set; }
        public int ExamTypeId { get; set; }
        public int LessonId { get; set; }
        public string TopicName { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ExamType ExamType { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
