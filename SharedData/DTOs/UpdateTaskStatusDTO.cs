using SharedData.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedData.DTOs
{
    public class UpdateTaskStatusDTO
    {
        [Required]
        public TaskItemStatus Status { get; set; }
    }
}
