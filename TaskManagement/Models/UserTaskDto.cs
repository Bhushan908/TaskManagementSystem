namespace TaskManagement.Models
{
	public class UserTaskDto
	{
		public int TaskId { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public DateTime DueDate { get; set; }
		public string? Status { get; set; }
		public int AssignedToUserId { get; set; }
		public ICollection<NoteDto> Notes { get; set; } = new List<NoteDto>();
		public ICollection<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
	}
}
