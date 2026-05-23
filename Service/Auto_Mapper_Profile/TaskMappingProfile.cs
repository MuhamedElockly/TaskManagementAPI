using AutoMapper;
using Domain.Entities.CoreEntities;
using SharedData.DTOs;
using DomainTaskStatus = Domain.Entities.CoreEntities.TaskItemStatus;
using DomainTaskPriority = Domain.Entities.CoreEntities.TaskPriority;
using SharedTaskStatus = SharedData.Enums.TaskItemStatus;
using SharedTaskPriority = SharedData.Enums.TaskPriority;

namespace Service.Auto_Mapper_Profile
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<CreateTaskDTO, TaskItem>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (DomainTaskStatus)s.Status))
                .ForMember(d => d.Priority, o => o.MapFrom(s => (DomainTaskPriority)s.Priority));

            CreateMap<TaskItem, TaskResultDTO>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (SharedTaskStatus)s.Status))
                .ForMember(d => d.Priority, o => o.MapFrom(s => (SharedTaskPriority)s.Priority));
        }
    }
}
