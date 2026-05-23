namespace Domain.Entities.CoreEntities
{
    public class TaskItem : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
