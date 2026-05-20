using Microsoft.EntityFrameworkCore;
using TaskManagement.Dto.ProjectDto;
using TaskManagement.Dto.ProjectTaskDto;
using TaskManagement.Models;
using TaskManagement.Repository.AccountRepository;

namespace TaskManagement.Repository.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDBContext _context;
        private readonly IAccountRepository _accountRepository;

        public ProjectRepository(AppDBContext context, IAccountRepository accountRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
        }

        public async Task<List<ProjectDetailsDto>> GetAllProjects(string userId)
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .Select(p => new ProjectDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UserId = userId,
                })
                .ToListAsync();
        }

        public async Task<ProjectDetailsDto?> GetProjectById(int id, string userId)
        {
            return await _context.Projects
                .Where(p => p.Id == id && p.UserId == userId)
                .Select(p => new ProjectDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UserId = userId,
                    Tasks = p.Tasks.Select(pt => new TaskDetailsDto
                    {
                        Id = pt.Id,
                        Title = pt.Title,
                        Description = pt.Description,
                        Status = pt.Status.ToString(),
                        Priority = pt.Priority.ToString(),
                        DueDate = pt.DueDate,
                        CreatedAt = pt.CreatedAt,
                        ProjectId = pt.ProjectId
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ProjectDetailsDto> CreateProject(CreateProjectDto createProjectDto, string userId)
        {
            var user = await _accountRepository.GetUserById(userId);
            // check if this user exists, if not return null
            if (user == null)
            {
                return null;
            }

            Project project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDetailsDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UserId = userId
            };
        }

        public async Task<bool> UpdateProject(int id, UpdateProjectDto updateProjectDto, string userId)
        {
            Project? project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (project == null) // if project not exist
            {
                return false;
            }

            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProject(int id, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Project? project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                if (project == null) // if project not exist
                {
                    return false;
                }

                var result = await _context.ProjectTasks
                    .Where(pt => pt.ProjectId == project.Id)
                    .ExecuteDeleteAsync();

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while deleting the project and its tasks.", ex);
            }
        }

    }
}
