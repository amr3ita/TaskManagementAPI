using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Dto.ProjectTaskDto;
using TaskManagement.Repository.ProjectTaskRepository;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskRepository _projectTaskRepository;

        public ProjectTaskController(IProjectTaskRepository projectTaskRepository)
        {
            _projectTaskRepository = projectTaskRepository;
        }
        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var projectTasks = await _projectTaskRepository.GetAllTasks(projectId, userId);
            if (projectTasks == null || projectTasks.Count == 0)
            {
                return NotFound();
            }

            return Ok(projectTasks);
        }

        [HttpGet("{id}", Name = "GetTaskById")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var task = await _projectTaskRepository.GetTaskById(id, userId);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var createdTask = await _projectTaskRepository.CreateTask(createTaskDto, userId);
            if (createdTask == null)
            {
                return NotFound("Failed to create task. Make sure the project exists and belongs to you.");
            }
            return CreatedAtRoute("GetTaskById", new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var updatedTask = await _projectTaskRepository.UpdateTask(id, updateTaskDto, userId);
            if (updatedTask == null)
            {
                return NotFound("Task not found or does not belong to you.");
            }
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var isDeleted = await _projectTaskRepository.DeleteTask(id, userId);
            if (!isDeleted)
            {
                return NotFound("Task not found or does not belong to you.");
            }

            return Ok(new { Message = "Task deleted successfully." });
        }

    }
}
