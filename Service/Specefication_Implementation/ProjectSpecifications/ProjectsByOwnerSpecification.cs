using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.ProjectSpecifications
{
    public class ProjectsByOwnerSpecification : Specification<Project, int>
    {
        public ProjectsByOwnerSpecification(string ownerId)
            : base(p => p.OwnerId == ownerId)
        {
            SetOrderByDescending(p => p.CreatedAt);
        }
    }
}
