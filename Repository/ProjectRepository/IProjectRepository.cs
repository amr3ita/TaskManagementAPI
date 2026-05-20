using TaskManagement.Dto.ProjectDto;

namespace TaskManagement.Repository.ProjectRepository
{
    public interface IProjectRepository
    {
        Task<List<ProjectDetailsDto>> GetAllProjects(string userId);
        Task<ProjectDetailsDto?> GetProjectById(int id, string userId);
        Task<ProjectDetailsDto> CreateProject(CreateProjectDto createProjectDto, string userId);
        Task<bool> UpdateProject(int id, UpdateProjectDto updateProjectDto, string userId);
        Task<bool> DeleteProject(int id, string userId);
    }
}
