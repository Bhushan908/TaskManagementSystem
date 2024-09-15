using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
	public class CreateUserDto
	{
		[Required(ErrorMessage = "First name is required.")]
		[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
		public string? FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
		public string? LastName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Role is required.")] // Enum for Employee, Manager, Admin
		public string? Role { get; set; }

		public int? ManagerId { get; set; } = null;
	}

}
