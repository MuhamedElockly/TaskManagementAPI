using SharedData.DTOs;
using SharedData.Wrapper;

namespace ServiceAbstraction
{
    public interface IProjectService
    {
        Task<ApiResponse<ProjectResultDTO>> CreateAsync(CreateProjectDTO dto);
        Task<ApiResponse<IEnumerable<ProjectResultDTO>>> GetAllAsync();
        Task<ApiResponse<ProjectResultDTO>> GetByIdAsync(int id);
        Task<ApiResponse<ProjectResultDTO>> UpdateAsync(int id, UpdateProjectDTO dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
