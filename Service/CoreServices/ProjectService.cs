using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CoreEntities;
using Microsoft.AspNetCore.Http;
using Service.Specefication_Implementation.ProjectSpecifications;
using ServiceAbstraction;
using SharedData.Constants;
using SharedData.DTOs;
using SharedData.Wrapper;
using System.Security.Claims;

namespace Service.CoreServices
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggingService _logger;
        private readonly IMapper _mapper;

        public ProjectService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILoggingService logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProjectResultDTO>> CreateAsync(CreateProjectDTO dto)
        {
            return await _logger.LogOperationAsync("Create project", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<ProjectResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var project = _mapper.Map<Project>(dto);
                project.CreatedAt = DateTime.UtcNow;
                project.OwnerId = ownerId;

                var repo = _unitOfWork.GetRepository<Project, int>();
                await repo.AddAsync(project);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<ProjectResultDTO>
                {
                    Success = true,
                    Message = "Project created successfully",
                    Data = _mapper.Map<ProjectResultDTO>(project)
                };
            });
        }

        public async Task<ApiResponse<IEnumerable<ProjectResultDTO>>> GetAllAsync()
        {
            return await _logger.LogOperationAsync("Get all projects", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<IEnumerable<ProjectResultDTO>>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var repo = _unitOfWork.GetRepository<Project, int>();
                var projects = await repo.GetAllAsynce(new ProjectsByOwnerSpecification(ownerId));

                return new ApiResponse<IEnumerable<ProjectResultDTO>>
                {
                    Success = true,
                    Message = "Projects retrieved successfully",
                    Data = _mapper.Map<IEnumerable<ProjectResultDTO>>(projects)
                };
            });
        }

        public async Task<ApiResponse<ProjectResultDTO>> GetByIdAsync(int id)
        {
            return await _logger.LogOperationAsync("Get project by id", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<ProjectResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var project = await GetOwnedProjectAsync(id, ownerId);
                if (project == null)
                {
                    return new ApiResponse<ProjectResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.ProjectNotFound,
                        Data = null
                    };
                }

                return new ApiResponse<ProjectResultDTO>
                {
                    Success = true,
                    Message = "Project retrieved successfully",
                    Data = _mapper.Map<ProjectResultDTO>(project)
                };
            });
        }

        public async Task<ApiResponse<ProjectResultDTO>> UpdateAsync(int id, UpdateProjectDTO dto)
        {
            return await _logger.LogOperationAsync("Update project", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<ProjectResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var project = await GetOwnedProjectAsync(id, ownerId);
                if (project == null)
                {
                    return new ApiResponse<ProjectResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.ProjectNotFound,
                        Data = null
                    };
                }

                _mapper.Map(dto, project);

                var repo = _unitOfWork.GetRepository<Project, int>();
                repo.Update(project);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<ProjectResultDTO>
                {
                    Success = true,
                    Message = "Project updated successfully",
                    Data = _mapper.Map<ProjectResultDTO>(project)
                };
            });
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            return await _logger.LogOperationAsync("Delete project", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = false
                    };
                }

                var project = await GetOwnedProjectAsync(id, ownerId);
                if (project == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = ResponseMessages.ProjectNotFound,
                        Data = false
                    };
                }

                var repo = _unitOfWork.GetRepository<Project, int>();
                await repo.DeleteAsync(project.Id);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Project deleted successfully",
                    Data = true
                };
            });
        }

        private async Task<Project?> GetOwnedProjectAsync(int id, string ownerId)
        {
            var repo = _unitOfWork.GetRepository<Project, int>();
            return await repo.GetFirstOrDefaultAsync(new ProjectByIdAndOwnerSpecification(id, ownerId));
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
