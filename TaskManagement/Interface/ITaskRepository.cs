using TaskManagement.Models;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Interface
{
	public interface ITaskRepository
	{
		Task<IEnumerable<UserTask>> GetAllTasksAsync();
		Task<UserTask> GetTaskByIdAsync(int taskId);
		Task AddTaskAsync(UserTask task);
		Task UpdateTaskAsync(UserTask task);
		Task DeleteTaskAsync(int taskId);
		Task SaveChangesAsync();
		Task AddNoteAsync(Note note);
		Task AddDocumentAsync(Document document);
		Task UpdateTaskStatusAsync(int taskId, TaskStatus status);
		Task<IEnumerable<UserTask>> GetTasksForManagerAsync(int managerId);
		Task<IEnumerable<UserTask>> GetTasksDueInWeekAsync();
		Task<IEnumerable<UserTask>> GetTasksDueInMonthAsync();
		Task<IEnumerable<UserTask>> GetTasksForWeekAsync(DateTime startOfWeek, DateTime endOfWeek);
		Task<IEnumerable<UserTask>> GetTasksForMonthAsync(DateTime startOfMonth, DateTime endOfMonth);
	}
}
