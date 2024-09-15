using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.Models;

namespace TaskManagement.Models
{
	public class UserTask
	{
		[Key]
		public int TaskId { get; set; }

		[Required]
		public string? Title { get; set; }

		public string? Description { get; set; }

		[Required]
		public DateTime DueDate { get; set; }

		public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

		public int AssignedToUserId { get; set; }
		public User? AssignedTo { get; set; }

		public ICollection<Note> Notes { get; set; } = new List<Note>();
		public ICollection<Document> Documents { get; set; } = new List<Document>();
	}




	public class Note
	{
		[Key]
		public int NoteId { get; set; }

		[Required]
		public int TaskId { get; set; } // Foreign key to the associated task

		public UserTask? Task { get; set; } // Navigation property to Task

		[Required]
		[StringLength(500)]
		public string? Content { get; set; } // Note content

		public DateTime CreatedDate { get; set; }
	}

	public class Document
	{
		[Key]
		public int DocumentId { get; set; }

		[Required]
		public int TaskId { get; set; } // Foreign key to the associated task

		public UserTask Task { get; set; } // Navigation property to Task

		[Required]
		[StringLength(200)]
		public string FileName { get; set; } // File name of the uploaded document

		[Column("Base64Data", TypeName = "TEXT")]
		public string Base64Data { get; set; } // Base64-encoded file data

		public DateTime UploadedDate { get; set; }
	}

	public enum TaskStatus
	{
		NotStarted,
		InProgress,
		Completed,
		OnHold
	}
}
