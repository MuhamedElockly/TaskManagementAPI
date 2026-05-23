using System.ComponentModel.DataAnnotations;

namespace SharedData.DTOs
{
    public class UpdateProjectDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
