using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
	public class UploadDocumentDto
	{
		[Required(ErrorMessage = "TaskId is required.")]
		public int TaskId { get; set; }

		[Required(ErrorMessage = "Base64Data is required.")]
		public string? Base64Data { get; set; } // Base64 encoded file data
	}
}