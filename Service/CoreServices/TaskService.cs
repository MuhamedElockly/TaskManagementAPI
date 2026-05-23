using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CoreEntities;
using Microsoft.AspNetCore.Http;
using Service.Specefication_Implementation.ProjectSpecifications;
using Service.Specefication_Implementation.TaskSpecifications;
using ServiceAbstraction;
using SharedData.Constants;
using SharedData.DTOs;
using SharedData.Wrapper;
using System.Security.Claims;
using DomainTaskStatus = Domain.Entities.CoreEntities.TaskItemStatus;

namespace Service.CoreServices
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggingService _logger;
        private readonly IMapper _mapper;

        public TaskService(
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

        public async Task<ApiResponse<TaskResultDTO>> CreateAsync(CreateTaskDTO dto)
        {
            return await _logger.LogOperationAsync("Create task", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<TaskResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var project = await GetOwnedProjectAsync(dto.ProjectId, ownerId);
                if (project == null)
                {
                    return new ApiResponse<TaskResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.ProjectNotFound,
                        Data = null
                    };
                }

                var task = _mapper.Map<TaskItem>(dto);

                var repo = _unitOfWork.GetRepository<TaskItem, int>();
                await repo.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<TaskResultDTO>
                {
                    Success = true,
                    Message = "Task created successfully",
                    Data = _mapper.Map<TaskResultDTO>(task)
                };
            });
        }

        public async Task<ApiResponse<TaskResultDTO>> UpdateStatusAsync(int id, UpdateTaskStatusDTO dto)
        {
            return await _logger.LogOperationAsync("Update task status", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<TaskResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var task = await GetOwnedTaskAsync(id, ownerId);
                if (task == null)
                {
                    return new ApiResponse<TaskResultDTO>
                    {
                        Success = false,
                        Message = ResponseMessages.TaskNotFound,
                        Data = null
                    };
                }

                task.Status = (DomainTaskStatus)dto.Status;

                var repo = _unitOfWork.GetRepository<TaskItem, int>();
                repo.Update(task);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<TaskResultDTO>
                {
                    Success = true,
                    Message = "Task status updated successfully",
                    Data = _mapper.Map<TaskResultDTO>(task)
                };
            });
        }

        public async Task<ApiResponse<IEnumerable<TaskResultDTO>>> GetByProjectAsync(int projectId)
        {
            return await _logger.LogOperationAsync("Get tasks by project", async () =>
            {
                var ownerId = GetCurrentUserId();
                if (ownerId == null)
                {
                    return new ApiResponse<IEnumerable<TaskResultDTO>>
                    {
                        Success = false,
                        Message = ResponseMessages.UserNotAuthenticated,
                        Data = null
                    };
                }

                var repo = _unitOfWork.GetRepository<TaskItem, int>();
                var tasks = await repo.GetAllAsynce(new TasksByProjectAndOwnerSpecification(projectId, ownerId));

                if (!tasks.Any())
                {
                    var project = await GetOwnedProjectAsync(projectId, ownerId);
                    if (project == null)
                    {
                        return new ApiResponse<IEnumerable<TaskResultDTO>>
                        {
                            Success = false,
                            Message = ResponseMessages.ProjectNotFound,
                            Data = null
                        };
                    }
                }

                return new ApiResponse<IEnumerable<TaskResultDTO>>
                {
                    Success = true,
                    Message = "Tasks retrieved successfully",
                    Data = _mapper.Map<IEnumerable<TaskResultDTO>>(tasks)
                };
            });
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            return await _logger.LogOperationAsync("Delete task", async () =>
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

                var task = await GetOwnedTaskAsync(id, ownerId);
                if (task == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = ResponseMessages.TaskNotFound,
                        Data = false
                    };
                }

                var repo = _unitOfWork.GetRepository<TaskItem, int>();
                await repo.DeleteAsync(task.Id);
                await _unitOfWork.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Task deleted successfully",
                    Data = true
                };
            });
        }

        private async Task<Project?> GetOwnedProjectAsync(int projectId, string ownerId)
        {
            var repo = _unitOfWork.GetRepository<Project, int>();
            return await repo.GetFirstOrDefaultAsync(new ProjectByIdAndOwnerSpecification(projectId, ownerId));
        }

        private async Task<TaskItem?> GetOwnedTaskAsync(int taskId, string ownerId)
        {
            var repo = _unitOfWork.GetRepository<TaskItem, int>();
            return await repo.GetFirstOrDefaultAsync(new TaskByIdAndOwnerSpecification(taskId, ownerId));
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
