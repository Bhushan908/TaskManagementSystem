using TaskManagement.Data;
using TaskManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Interface;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly TaskManagementContext _context;

		public TaskRepository(TaskManagementContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<UserTask>> GetAllTasksAsync()
		{
			return await _context.UserTasks
				   .Include(t => t.AssignedTo) // Include the AssignedTo navigation property
				   .Include(t => t.Notes) // Include the Notes navigation property
				   .Include(t => t.Documents) // Include the Documents navigation property
				   .ToListAsync();
		}

		public async Task<UserTask> GetTaskByIdAsync(int taskId)
		{
			return await _context.UserTasks
				.Include(t => t.Notes)
				.Include(t => t.Documents)
				.FirstOrDefaultAsync(t => t.TaskId == taskId);
		}

		public async Task AddTaskAsync(UserTask task)
		{
			await _context.UserTasks.AddAsync(task);
		}

		public async Task UpdateTaskAsync(UserTask task)
		{
			_context.UserTasks.Update(task);
		}

		public async Task DeleteTaskAsync(int taskId)
		{
			var task = await GetTaskByIdAsync(taskId);
			if (task != null)
			{
				_context.UserTasks.Remove(task);
			}
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task AddNoteAsync(Note note)
		{
			await _context.Notes.AddAsync(note);
		}

		public async Task AddDocumentAsync(Document document)
		{
			await _context.Documents.AddAsync(document);
		}

		public async Task UpdateTaskStatusAsync(int taskId, TaskStatus status)
		{
			var task = await GetTaskByIdAsync(taskId);
			if (task != null)
			{
				task.Status = status;
				await SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<UserTask>> GetTasksForManagerAsync(int managerId)
		{
			return await _context.UserTasks
				.Where(t => t.AssignedTo.ManagerId == managerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<UserTask>> GetTasksDueInWeekAsync()
		{
			var currentDate = DateTime.UtcNow;
			var nextWeekDate = currentDate.AddDays(7);

			return await _context.UserTasks.Include(t => t.Notes)
				.Include(t => t.Documents)
			  .Where(t => t.DueDate >= currentDate && t.DueDate <= nextWeekDate && t.Status != TaskStatus.Completed)
			  .ToListAsync();
		}

		public async Task<IEnumerable<UserTask>> GetTasksDueInMonthAsync()
		{
			var currentDate = DateTime.UtcNow;
			var nextMonthDate = currentDate.AddMonths(1);

			return await _context.UserTasks.Include(t => t.Notes)
				.Include(t => t.Documents)
				.Where(t => t.DueDate >= currentDate && t.DueDate <= nextMonthDate && t.Status != TaskStatus.Completed)
				.ToListAsync();
		}
		public async Task<IEnumerable<UserTask>> GetTasksForWeekAsync(DateTime startOfWeek, DateTime endOfWeek)
		{
			return await _context.UserTasks
				.Where(t => t.DueDate >= startOfWeek && t.DueDate <= endOfWeek)
				.ToListAsync();
		}

		public async Task<IEnumerable<UserTask>> GetTasksForMonthAsync(DateTime startOfMonth, DateTime endOfMonth)
		{
			return await _context.UserTasks
				.Where(t => t.DueDate >= startOfMonth && t.DueDate <= endOfMonth)
				.ToListAsync();
		}

	}
}
