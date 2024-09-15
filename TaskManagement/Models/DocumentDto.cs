namespace TaskManagement.Models
{
	public class DocumentDto
	{
		public int TaskId { get; set; }
		public int DocumentId { get; set; }
		public string? FileName { get; set; }
		public string? Base64Data { get; set; }
		public DateTime UploadedDate { get; set; }
	}
}
