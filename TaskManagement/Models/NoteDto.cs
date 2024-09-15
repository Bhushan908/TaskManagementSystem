namespace TaskManagement.Models
{
	public class NoteDto
	{
		public int TaskId { get; set; }
		public int NoteId { get; set; }
		public string? Content { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
