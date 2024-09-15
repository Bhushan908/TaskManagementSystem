using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
	public class TaskManagementContext : DbContext
	{
		public TaskManagementContext(DbContextOptions<TaskManagementContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<UserTask> UserTasks { get; set; }
		public DbSet<Note> Notes { get; set; }
		public DbSet<Document> Documents { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
			  .Property(u => u.Role)
			  .HasConversion(
				  v => v, // No conversion needed for string
				  v => v  // No conversion needed for string
			  );

			modelBuilder.Entity<User>()
				.HasMany(u => u.Tasks)
				.WithOne(t => t.AssignedTo)
				.HasForeignKey(t => t.AssignedToUserId);

			modelBuilder.Entity<UserTask>()
				.HasMany(t => t.Notes)
				.WithOne(n => n.Task)
				.HasForeignKey(n => n.TaskId);

			modelBuilder.Entity<UserTask>()
				.HasMany(t => t.Documents)
				.WithOne(d => d.Task)
				.HasForeignKey(d => d.TaskId);

			// Self-referencing foreign key
			modelBuilder.Entity<User>()
				.HasOne(u => u.Manager)
				.WithMany()
				.HasForeignKey(u => u.ManagerId)
				.OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete
		}
	}
}