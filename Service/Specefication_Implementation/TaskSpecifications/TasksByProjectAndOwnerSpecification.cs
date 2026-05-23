using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.TaskSpecifications
{
    public class TasksByProjectAndOwnerSpecification : Specification<TaskItem, int>
    {
        public TasksByProjectAndOwnerSpecification(int projectId, string ownerId)
            : base(t => t.ProjectId == projectId && t.Project.OwnerId == ownerId)
        {
            Includes.Add(t => t.Project);
            SetOrderByDescending(t => t.DueDate);
        }
    }
}
