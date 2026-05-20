using Microsoft.EntityFrameworkCore;
using TaskManagement.Dto.ProjectTaskDto;
using TaskManagement.Models;

namespace TaskManagement.Repository.ProjectTaskRepository
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private readonly AppDBContext _context;

        public ProjectTaskRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<TaskDetailsDto>> GetAllTasks(int projectId, string userId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(pt => pt.Id == projectId && pt.UserId == userId);
            if (!projectExists)
            {
                return new List<TaskDetailsDto>(); // Project not found or user does not have access
            }

            return await _context.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskDetailsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId
                }).ToListAsync();
        }
        public async Task<TaskDetailsDto?> GetTaskById(int taskId, string userId)
        {

            return await _context.ProjectTasks
                .Include(t => t.Project)
                .Where(t => t.Id == taskId && t.Project.UserId == userId)
                .Select(t => new TaskDetailsDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    ProjectId = t.ProjectId
                })
                .FirstOrDefaultAsync();
        }
        public async Task<TaskDetailsDto> CreateTask(CreateTaskDto createTaskDto, string userId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(pt => pt.Id == createTaskDto.ProjectId && pt.UserId == userId);

            if (!projectExists)
            {
                return null; // Project not found or user does not have access
            }

            ProjectTask task = new ProjectTask
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                ProjectId = createTaskDto.ProjectId,
            };

            _context.ProjectTasks.Add(task);
            await _context.SaveChangesAsync();

            // return created task details
            return new TaskDetailsDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId
            };
        }
        public async Task<TaskDetailsDto?> UpdateTask(int id, UpdateTaskDto updateTaskDto, string userId)
        {
            // check if task exists
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null) return null;

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.Priority = updateTaskDto.Priority;
            task.DueDate = updateTaskDto.DueDate;

            await _context.SaveChangesAsync();

            return new TaskDetailsDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId
            };
        }
        public async Task<bool> DeleteTask(int id, string userId)
        {
            // check if task exist
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null) return false;

            _context.ProjectTasks.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
