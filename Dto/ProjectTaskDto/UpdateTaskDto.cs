using System.ComponentModel.DataAnnotations;
using TaskManagement.Enums;

namespace TaskManagement.Dto.ProjectTaskDto
{
    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(100, ErrorMessage = "Title max length is 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description max length is 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task status is required.")]
        public TaskStatusEnum Status { get; set; }

        [Required(ErrorMessage = "Task priority is required.")]
        public TaskPriorityEnum Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
