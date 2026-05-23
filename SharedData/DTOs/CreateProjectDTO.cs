using System.ComponentModel.DataAnnotations;

namespace SharedData.DTOs;

/// <summary>Request body for creating a new project.</summary>
public class CreateProjectDTO
{
    /// <summary>Project display name (max 200 characters).</summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional project description (max 1000 characters).</summary>
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
}
