using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.ProjectSpecifications
{
    public class ProjectByIdAndOwnerSpecification : Specification<Project, int>
    {
        public ProjectByIdAndOwnerSpecification(int projectId, string ownerId)
            : base(p => p.Id == projectId && p.OwnerId == ownerId)
        {
        }
    }
}
