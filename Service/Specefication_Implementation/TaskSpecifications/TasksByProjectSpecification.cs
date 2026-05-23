using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.TaskSpecifications
{
    public class TasksByProjectSpecification : Specification<TaskItem, int>
    {
        public TasksByProjectSpecification(int projectId)
            : base(t => t.ProjectId == projectId)
        {
            SetOrderByDescending(t => t.DueDate);
        }
    }
}
