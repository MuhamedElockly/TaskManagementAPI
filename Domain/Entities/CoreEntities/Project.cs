using Domain.Entities.IdentityEntity;

namespace Domain.Entities.CoreEntities
{
    public class Project : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
