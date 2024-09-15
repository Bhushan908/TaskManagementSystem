using TaskManagement.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Interface
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllUsersAsync();
		Task<User> GetUserByIdAsync(int userId);
		Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task DeleteUserAsync(int userId);
		Task SaveChangesAsync();
	}
}
