using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
	public class User
	{
		public int UserId { get; set; }

		[Required(ErrorMessage = "First name is required.")]
		[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
		public string? FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Role is required.")]
		public string? Role { get; set; } // Enum for Employee, Manager, Admin

		public int? ManagerId { get; set; }
		public User? Manager { get; set; }
		public ICollection<UserTask> Tasks { get; set; } = new List<UserTask>();
	}


	public enum UserRole
	{
		Employee,
		Manager,
		Admin
	}
}

