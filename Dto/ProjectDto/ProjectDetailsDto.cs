using TaskManagement.Dto.ProjectTaskDto;

namespace TaskManagement.Dto.ProjectDto
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public List<TaskDetailsDto> Tasks { get; set; } = new List<TaskDetailsDto>();
    }
}
