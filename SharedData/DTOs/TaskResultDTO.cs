using SharedData.Enums;

namespace SharedData.DTOs
{
    public class TaskResultDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public int ProjectId { get; set; }
    }
}
