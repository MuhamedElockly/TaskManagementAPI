using SharedData.DTOs;
using SharedData.Wrapper;

namespace ServiceAbstraction
{
    public interface ITaskService
    {
        Task<ApiResponse<TaskResultDTO>> CreateAsync(CreateTaskDTO dto);
        Task<ApiResponse<TaskResultDTO>> UpdateStatusAsync(int id, UpdateTaskStatusDTO dto);
        Task<ApiResponse<IEnumerable<TaskResultDTO>>> GetByProjectAsync(int projectId);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
