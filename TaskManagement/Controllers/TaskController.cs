using Microsoft.AspNetCore.Mvc;
using TaskManagement.Interface;
using TaskManagement.Models;
using TaskStatus = TaskManagement.Models.TaskStatus;

namespace TaskManagement.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TaskController : ControllerBase
	{
		private readonly ITaskRepository _taskRepository;

		public TaskController(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		/// <summary>
		/// It is use to get all task list.
		/// </summary>
		/// <returns></returns>
		[HttpGet("GetAllTasks")]
		public async Task<IActionResult> GetAllTasks()
		{
			// Initialize the taskDtos list
			List<UserTaskDto> taskDtos = new();
			try
			{
				var tasks = await _taskRepository.GetAllTasksAsync();

				taskDtos = tasks.Select(task => new UserTaskDto
				{
					TaskId = task.TaskId,
					Title = task.Title,
					Description = task.Description,
					DueDate = task.DueDate,
					Status = task.Status.ToString(), // Convert enum to string,
					AssignedToUserId = task.AssignedToUserId,
					Notes = task.Notes.Select(note => new NoteDto
					{
						NoteId = note.NoteId,
						TaskId = note.TaskId,
						Content = note.Content,
						CreatedDate = note.CreatedDate
					}).ToList(),
					Documents = task.Documents.Select(doc => new DocumentDto
					{
						TaskId = doc.TaskId,
						DocumentId = doc.DocumentId,
						FileName = doc.FileName,
						Base64Data = doc.Base64Data,
						UploadedDate = doc.UploadedDate
					}).ToList()
				}).ToList();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the tasks. {ex.Message}");
			}

			return Ok(taskDtos);
		}

		/// <summary>
		/// It is use to get task by ID.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("GetTaskById/{id}")]
		public async Task<IActionResult> GetTaskById(int id)
		{
			UserTaskDto taskDto = new();
			try
			{
				var task = await _taskRepository.GetTaskByIdAsync(id);
				if (task == null)
					return NotFound();

				taskDto = new UserTaskDto
				{
					TaskId = task.TaskId,
					Title = task.Title,
					Description = task.Description,
					DueDate = task.DueDate,
					Status = task.Status.ToString(), // Convert enum to string,
					AssignedToUserId = task.AssignedToUserId,
					Notes = task.Notes.Select(n => new NoteDto
					{
						TaskId = n.TaskId,
						NoteId = n.NoteId,
						Content = n.Content,
						CreatedDate = n.CreatedDate
					}).ToList(),
					Documents = task.Documents.Select(d => new DocumentDto
					{
						TaskId = d.TaskId,
						DocumentId = d.DocumentId,
						FileName = d.FileName,
						Base64Data = d.Base64Data,
						UploadedDate = d.UploadedDate
					}).ToList()
				};
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the task. {ex.Message}");
			}
			return Ok(taskDto);
		}

		/// <summary>
		/// It is use to create a new task.
		/// </summary>
		/// <param name="taskDto"></param>
		/// <returns></returns>
		[HttpPost("CreateTask")]
		public async Task<IActionResult> CreateTask([FromBody] CreateUserTaskDto taskDto)
		{
			UserTask task = new();
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				task = new UserTask
				{
					Title = taskDto.Title,
					Description = taskDto.Description,
					DueDate = taskDto.DueDate,
					AssignedToUserId = taskDto.AssignedToUserId
				};

				await _taskRepository.AddTaskAsync(task);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while creating the tasks. {ex.Message}");
			}

			return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskId }, task);
		}

		/// <summary>
		/// It is use to update an existing task.
		/// </summary>
		/// <param name="updateUserTaskDto"></param>
		/// <returns></returns>
		[HttpPut("UpdateTask/{id}")]
		public async Task<IActionResult> UpdateTask(int id, CreateUserTaskDto updateUserTaskDto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var task = new UserTask
				{
					TaskId = id,
					Title = updateUserTaskDto.Title,
					Description = updateUserTaskDto.Description,
					DueDate = updateUserTaskDto.DueDate,
					AssignedToUserId = updateUserTaskDto.AssignedToUserId
				};

				await _taskRepository.UpdateTaskAsync(task);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while updating the tasks. {ex.Message}");
			}

			return Ok($"Task {updateUserTaskDto.Title} has been successfully updated.");
		}

		/// <summary>
		/// It is use to delete a task.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("DeleteTask/{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			try
			{
				await _taskRepository.DeleteTaskAsync(id);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while deleting the tasks. {ex.Message}");
			}
			return Ok($"Task {id} status has been successfully deleted.");
		}

		/// <summary>
		/// It is use to add notes to task.
		/// </summary>
		/// <param name="createNoteDto"></param>
		/// <returns></returns>
		[HttpPost("add-note")]
		public async Task<IActionResult> AddNoteToTask([FromBody] CreateNoteDto createNoteDto)
		{
			Note note = new();
			try
			{
				note = new Note { TaskId = createNoteDto.TaskId, Content = createNoteDto.Content };
				await _taskRepository.AddNoteAsync(note);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while adding note to the task. {ex.Message}");
			}
			return Ok(note);
		}

		/// <summary>
		/// It is use to Upload a document to a task.
		/// </summary>
		/// <param name="documentDto"></param>
		/// <returns></returns>
		[HttpPost("upload-document")]
		public async Task<IActionResult> UploadDocumentToTask([FromBody] UploadDocumentDto documentDto)
		{
			Document document = new();
			try
			{
				// Validate base64 input
				if (string.IsNullOrWhiteSpace(documentDto.Base64Data))
					return BadRequest("Base64 data is required.");

				// Create the document entity with base64 data
				document = new Document
				{
					TaskId = documentDto.TaskId,
					FileName = $"document_{documentDto.TaskId}_{Guid.NewGuid()}", // Generating a unique file name
					Base64Data = documentDto.Base64Data, // Save the base64 data
					UploadedDate = DateTime.UtcNow
				};

				// Save the document entity to the database
				await _taskRepository.AddDocumentAsync(document);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while uploading document for the task. {ex.Message}");
			}

			return Ok(document);
		}

		/// <summary>
		/// It is use to update a task status.
		/// </summary>
		/// <param name="taskId"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[HttpPut("{taskId}/UpdateTaskStatus")]
		public async Task<IActionResult> UpdateTaskStatus(int taskId, [FromBody] string status)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(status))
					return BadRequest("Status cannot be empty.");

				// Try to parse the input string to a TaskStatus enum value
				if (!Enum.TryParse<TaskStatus>(status, true, out var taskStatus))
					return BadRequest("Invalid status value. Allowed values are: NotStarted, InProgress, Completed, OnHold.");

				await _taskRepository.UpdateTaskStatusAsync(taskId, taskStatus);
				await _taskRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while updating the task status. {ex.Message}");
			}

			return Ok($"Task {taskId} status has been successfully updated.");
		}

		/// <summary>
		/// It is use to get task list for a manager.
		/// </summary>
		/// <param name="managerId"></param>
		/// <returns></returns>
		[HttpGet("manager/{managerId}")]
		public async Task<IActionResult> GetTasksForManager(int managerId)
		{
			List<UserTask> taskList = new();
			try
			{
				taskList = (List<UserTask>)await _taskRepository.GetTasksForManagerAsync(managerId);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while getting the tasks list for the manager. {ex.Message}");
			}
			return Ok(taskList);
		}

		/// <summary>
		/// It is use to get tasks due in the next week.
		/// </summary>
		/// <returns></returns>
		[HttpGet("due-in-week")]
		public async Task<IActionResult> GetTasksDueInWeek()
		{
			List<UserTaskDto> taskDtos = new();
			try
			{
				var tasks = await _taskRepository.GetTasksDueInWeekAsync();

				taskDtos = tasks.Select(task => new UserTaskDto
				{
					TaskId = task.TaskId,
					Title = task.Title,
					Description = task.Description,
					DueDate = task.DueDate,
					Status = task.Status.ToString(), // Convert enum to string,
					AssignedToUserId = task.AssignedToUserId,
					Notes = task.Notes.Select(note => new NoteDto
					{
						NoteId = note.NoteId,
						TaskId = note.TaskId,
						Content = note.Content,
						CreatedDate = note.CreatedDate
					}).ToList(),
					Documents = task.Documents.Select(doc => new DocumentDto
					{
						TaskId = doc.TaskId,
						DocumentId = doc.DocumentId,
						FileName = doc.FileName,
						Base64Data = doc.Base64Data,
						UploadedDate = doc.UploadedDate
					}).ToList()
				}).ToList();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the tasks. {ex.Message}");
			}
			return Ok(taskDtos);
		}

		/// <summary>
		/// It is use to get due in the next week.
		/// </summary>
		/// <returns></returns>
		[HttpGet("due-in-month")]
		public async Task<IActionResult> GetTasksDueInMonth()
		{
			List<UserTaskDto> taskDtos = new();
			try
			{
				var tasks = await _taskRepository.GetTasksDueInMonthAsync();

				taskDtos = tasks.Select(task => new UserTaskDto
				{
					TaskId = task.TaskId,
					Title = task.Title,
					Description = task.Description,
					DueDate = task.DueDate,
					Status = task.Status.ToString(), // Convert enum to string,
					AssignedToUserId = task.AssignedToUserId,
					Notes = task.Notes.Select(note => new NoteDto
					{
						NoteId = note.NoteId,
						TaskId = note.TaskId,
						Content = note.Content,
						CreatedDate = note.CreatedDate
					}).ToList(),
					Documents = task.Documents.Select(doc => new DocumentDto
					{
						TaskId = doc.TaskId,
						DocumentId = doc.DocumentId,
						FileName = doc.FileName,
						Base64Data = doc.Base64Data,
						UploadedDate = doc.UploadedDate
					}).ToList()
				}).ToList();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the tasks. {ex.Message}");
			}
			return Ok(taskDtos);
		}

		/// <summary>
		/// It is use to get task status report.
		/// </summary>
		/// <param name="period"></param>
		/// <returns></returns>
		[HttpGet("GetTaskStatusReport")]
		public async Task<IActionResult> GetTaskStatusReport([FromQuery] string period)
		{
			TaskStatusReportDto taskStatusReportDto = new();
			try
			{
				IEnumerable<UserTask> tasks;

				if (period.Equals("week", StringComparison.OrdinalIgnoreCase))
				{
					var startOfWeek = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);
					var endOfWeek = startOfWeek.AddDays(7);
					tasks = await _taskRepository.GetTasksForWeekAsync(startOfWeek, endOfWeek);
				}
				else if (period.Equals("month", StringComparison.OrdinalIgnoreCase))
				{
					var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
					var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
					tasks = await _taskRepository.GetTasksForMonthAsync(startOfMonth, endOfMonth);
				}
				else
				{
					return BadRequest("Invalid period. Please specify 'week' or 'month'.");
				}

				if (!tasks.Any())
					return Ok(new TaskStatusReportDto()); // No tasks found, return empty report.

				var totalTasks = tasks.Count();
				var notStartedCount = tasks.Count(t => t.Status == TaskStatus.NotStarted);
				var inProgressCount = tasks.Count(t => t.Status == TaskStatus.InProgress);
				var completedCount = tasks.Count(t => t.Status == TaskStatus.Completed);
				var onHoldCount = tasks.Count(t => t.Status == TaskStatus.OnHold);

				taskStatusReportDto = new TaskStatusReportDto
				{
					TotalTask = totalTasks,
					NotStartedPercentage = (double)notStartedCount / totalTasks * 100,
					InProgressPercentage = (double)inProgressCount / totalTasks * 100,
					CompletedPercentage = (double)completedCount / totalTasks * 100,
					OnHoldPercentage = (double)onHoldCount / totalTasks * 100
				};
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while fetching the tasks status report. {ex.Message}");
			}

			return Ok(taskStatusReportDto);
		}
	}
}