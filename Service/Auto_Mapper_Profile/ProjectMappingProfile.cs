using AutoMapper;
using Domain.Entities.CoreEntities;
using SharedData.DTOs;

namespace Service.Auto_Mapper_Profile
{
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            CreateMap<CreateProjectDTO, Project>();
            CreateMap<UpdateProjectDTO, Project>();
            CreateMap<Project, ProjectResultDTO>();
        }
    }
}
