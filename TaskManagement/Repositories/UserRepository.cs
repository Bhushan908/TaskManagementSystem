using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Interface;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly TaskManagementContext _context;

		public UserRepository(TaskManagementContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<User?> GetUserByIdAsync(int userId)
		{
			return await _context.Users.FindAsync(userId);
		}
		public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
		{
			return await _context.Users
				.Where(u => u.Role == role)
				.ToListAsync();
		}

		public async Task AddUserAsync(User user)
		{
			await _context.Users.AddAsync(user);
		}

		public async Task UpdateUserAsync(User user)
		{
			_context.Entry(user).State = EntityState.Modified;
		}

		public async Task DeleteUserAsync(int userId)
		{
			var user = await _context.Users.FindAsync(userId);
			if (user != null)
			{
				_context.Users.Remove(user);
			}
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}

}
