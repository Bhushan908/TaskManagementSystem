using Microsoft.AspNetCore.Mvc;
using TaskManagement.Models;
using TaskManagement.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskManagement.Interface;

namespace TaskManagement.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository _userRepository;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		/// <summary>
		/// It is use to get all users list.
		/// </summary>
		/// <returns></returns>
		[HttpGet("GetAllUsers")]
		public async Task<IActionResult> GetAllUsers()
		{
			List<UserDto> userDto = new();
			try
			{
				var users = await _userRepository.GetAllUsersAsync();
				userDto = users.Select(task => new UserDto
				{
					UserId = task.UserId,
					FirstName = task.FirstName,
					LastName = task.LastName,
					Email = task.Email,
					ManagerId = task.ManagerId,
					Role = task.Role
				}).ToList();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the users list. {ex.Message}");
			}
			return Ok(userDto);
		}

		/// <summary>
		/// It is use to get user details based on id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("GetUserById/{id}")]
		public async Task<IActionResult> GetUserById(int id)
		{
			UserDto userDto = new();
			try
			{
				var user = await _userRepository.GetUserByIdAsync(id);
				if (user == null)
					return NotFound("User not found");
				userDto = new UserDto
				{
					UserId = user.UserId,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					ManagerId = user.ManagerId,
					Role = user.Role
				};
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the user details based on {id} ID. {ex.Message}");
			}

			return Ok(userDto);
		}

		/// <summary>
		/// It is use to get user list based on the role.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		[HttpGet("GetUsersByRole/{role}")]
		public async Task<IActionResult> GetUsersByRole(string role)
		{
			List<UserDto> userDto = new();
			try
			{
				var users = await _userRepository.GetUsersByRoleAsync(role);
				userDto = users.Select(task => new UserDto
				{
					UserId = task.UserId,
					FirstName = task.FirstName,
					LastName = task.LastName,
					Email = task.Email,
					ManagerId = task.ManagerId,
					Role = task.Role
				}).ToList();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the user details based on role. {ex.Message}");
			}
			return Ok(userDto);
		}

		/// <summary>
		/// it use to create user.
		/// </summary>
		/// <param name="userDto"></param>
		/// <returns></returns>
		[HttpPost("CreateUser")]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
		{
			User user = new();
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				// Validate and convert role string to enum if necessary
				if (!Enum.TryParse<UserRole>(userDto.Role, true, out var roleEnum))
					return BadRequest("Invalid role value.");

				// Check if the manager exists if ManagerId is provided
				if (userDto.ManagerId.HasValue && userDto.Role != "Manager")
				{
					var manager = await _userRepository.GetUserByIdAsync(userDto.ManagerId.Value);
					if (manager == null)
						return BadRequest("Manager ID is invalid.");
				}

				user = new User
				{
					FirstName = userDto.FirstName,
					LastName = userDto.LastName,
					Email = userDto.Email,
					Role = userDto.Role,
					ManagerId = userDto.ManagerId
				};

				await _userRepository.AddUserAsync(user);
				await _userRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while creating the user. {ex.Message}");
			}

			return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
		}

		/// <summary>
		/// It is use to update user details.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userDto"></param>
		/// <returns></returns>
		[HttpPut("UpdateUser/{id}")]
		public async Task<IActionResult> UpdateUser(int id, CreateUserDto userDto)
		{
			try
			{
				if (id == userDto.ManagerId)
					return BadRequest("User id and manager id must be different.");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var user = new User
				{
					UserId = id,
					Email = userDto.Email,
					FirstName = userDto.FirstName,
					LastName = userDto.LastName,
					ManagerId = userDto.ManagerId,
					Role = userDto.Role
				};

				await _userRepository.UpdateUserAsync(user);
				await _userRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while updating the user. {ex.Message}");
			}
			return Ok($"User {userDto.Email} has been successfully updated.");
		}

		/// <summary>
		/// It is use to delete user.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("DeleteUser/{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				await _userRepository.DeleteUserAsync(id);
				await _userRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while deleting the user. {ex.Message}");
			}
			return Ok($"User {id} has been successfully deleted.");
		}
	}
}