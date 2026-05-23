using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using SharedData.Constants;
using SharedData.DTOs;
using SharedData.Wrapper;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILoggingService _logger;

        public TaskController(ITaskService taskService, ILoggingService loggingService)
        {
            _taskService = taskService;
            _logger = loggingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTaskDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponse<string>.FailResponse("Task data is required"));
                }

                if (!ModelState.IsValid)
                {
                    var errorDetails = ModelState
                        .Where(ms => ms.Value!.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(ApiResponse<object>.FailResponse("Validation failed", errorDetails));
                }

                var response = await _taskService.CreateAsync(dto);

                if (response.Success)
                {
                    _logger.LogInformation("Task created successfully");
                    return Ok(response);
                }

                if (response.Message == ResponseMessages.UserNotAuthenticated)
                {
                    return Unauthorized(response);
                }

                if (response.Message == ResponseMessages.ProjectNotFound)
                {
                    return NotFound(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while creating the task."));
            }
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromBody] UpdateTaskStatusDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ApiResponse<string>.FailResponse("Task status data is required"));
                }

                if (!ModelState.IsValid)
                {
                    var errorDetails = ModelState
                        .Where(ms => ms.Value!.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return BadRequest(ApiResponse<object>.FailResponse("Validation failed", errorDetails));
                }

                var response = await _taskService.UpdateStatusAsync(id, dto);

                if (response.Success)
                {
                    _logger.LogInformation("Task status updated successfully");
                    return Ok(response);
                }

                if (response.Message == ResponseMessages.UserNotAuthenticated)
                {
                    return Unauthorized(response);
                }

                if (response.Message == ResponseMessages.TaskNotFound)
                {
                    return NotFound(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while updating the task status."));
            }
        }

        [HttpGet("project/{projectId:int}")]
        public async Task<IActionResult> GetByProjectAsync(int projectId)
        {
            try
            {
                var response = await _taskService.GetByProjectAsync(projectId);

                if (response.Success)
                {
                    _logger.LogInformation("Tasks retrieved successfully");
                    return Ok(response);
                }

                if (response.Message == ResponseMessages.UserNotAuthenticated)
                {
                    return Unauthorized(response);
                }

                if (response.Message == ResponseMessages.ProjectNotFound)
                {
                    return NotFound(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while retrieving tasks."));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var response = await _taskService.DeleteAsync(id);

                if (response.Success)
                {
                    _logger.LogInformation("Task deleted successfully");
                    return Ok(response);
                }

                if (response.Message == ResponseMessages.UserNotAuthenticated)
                {
                    return Unauthorized(response);
                }

                if (response.Message == ResponseMessages.TaskNotFound)
                {
                    return NotFound(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, ApiResponse<string>.FailResponse("An error occurred while deleting the task."));
            }
        }
    }
}
