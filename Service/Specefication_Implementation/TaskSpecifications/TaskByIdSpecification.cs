using Domain.Entities.CoreEntities;

namespace Service.Specefication_Implementation.TaskSpecifications
{
    public class TaskByIdSpecification : Specification<TaskItem, int>
    {
        public TaskByIdSpecification(int taskId)
            : base(t => t.Id == taskId)
        {
            Includes.Add(t => t.Project);
        }
    }
}
