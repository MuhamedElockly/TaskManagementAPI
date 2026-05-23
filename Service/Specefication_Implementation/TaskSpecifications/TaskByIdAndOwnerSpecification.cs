using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.TaskSpecifications
{
    public class TaskByIdAndOwnerSpecification : Specification<TaskItem, int>
    {
        public TaskByIdAndOwnerSpecification(int taskId, string ownerId)
            : base(t => t.Id == taskId && t.Project.OwnerId == ownerId)
        {
            Includes.Add(t => t.Project);
        }
    }
}
