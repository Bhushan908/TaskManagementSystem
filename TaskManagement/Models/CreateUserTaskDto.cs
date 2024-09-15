using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
	public class CreateUserTaskDto
	{
		[Required(ErrorMessage ="Title is required.")]
		[StringLength(50, ErrorMessage = "Title can not be loger than 50 characters.")]
		public string? Title { get; set; }

		[Required(ErrorMessage ="Description is required")]
		[StringLength(100, ErrorMessage = "Description can not be loger than 100 characters.")]
		public string? Description { get; set; }

		[Required]
		public DateTime DueDate { get; set; }

		[Required(ErrorMessage = "AssignedToUserId is required.")]
		public int AssignedToUserId { get; set; }
	}

}
