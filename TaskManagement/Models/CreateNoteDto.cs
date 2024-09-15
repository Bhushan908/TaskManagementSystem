using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
	public class CreateNoteDto
	{
		[Required(ErrorMessage = "TaskId is required.")]
		public int TaskId { get; set; }

		[Required(ErrorMessage = "Content is required.")]
		[StringLength(200, ErrorMessage = "Content can not be loger than 200 characters.")]
		public string? Content { get; set; }
	}
}