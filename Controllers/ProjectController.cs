using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Dto.ProjectDto;
using TaskManagement.Repository.ProjectRepository;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {

            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var projects = await _projectRepository.GetAllProjects(userId);
            return Ok(projects);
        }

        [HttpGet("{id}", Name = "GetProjectById")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var project = await _projectRepository.GetProjectById(id, userId);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto createProjectDto)
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

            var project = await _projectRepository.CreateProject(createProjectDto, userId);
            // if any error happens
            if (project == null)
            {
                return BadRequest("Failed to create project");
            }

            return CreatedAtAction("GetProjectById", new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto updateProjectDto)
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

            bool isUpdated = await _projectRepository.UpdateProject(id, updateProjectDto, userId);
            if (!isUpdated)
            {
                return NotFound(new { Message = "Project not found or you don't have access to modify it." });
            }
            return Ok(new { Message = "Project updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            string? userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            bool isDeleted = await _projectRepository.DeleteProject(id, userId);
            if (!isDeleted)
            {
                return NotFound(new { Message = "Project not found or you don't have access to delete it." });
            }
            return Ok(new { Message = "Project deleted successfully." });
        }
    }
}
