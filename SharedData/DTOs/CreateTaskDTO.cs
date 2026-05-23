using SharedData.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedData.DTOs
{
    public class CreateTaskDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

        public DateTime DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required]
        public int ProjectId { get; set; }
    }
}
