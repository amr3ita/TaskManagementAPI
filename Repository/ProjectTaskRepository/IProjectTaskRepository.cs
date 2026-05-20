using TaskManagement.Dto.ProjectTaskDto;

namespace TaskManagement.Repository.ProjectTaskRepository
{
    public interface IProjectTaskRepository
    {
        Task<List<TaskDetailsDto>> GetAllTasks(int projectId, string userId);
        Task<TaskDetailsDto?> GetTaskById(int taskId, string userId);
        Task<TaskDetailsDto> CreateTask(CreateTaskDto createTaskDto, string userId);
        Task<TaskDetailsDto?> UpdateTask(int id, UpdateTaskDto updateTaskDto, string userId);
        Task<bool> DeleteTask(int id, string userId);
    }
}
