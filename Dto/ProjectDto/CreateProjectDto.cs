using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Dto.ProjectDto
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(100, ErrorMessage = "Project name Max Lenght is 100 characters.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Project Description Max Lenght is 500 characters.")]
        public string Description { get; set; }
    }
}
