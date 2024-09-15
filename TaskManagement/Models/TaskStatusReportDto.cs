namespace TaskManagement.Models
{
	public class TaskStatusReportDto
	{
		public int TotalTask {  get; set; }
		public double NotStartedPercentage { get; set; }
		public double InProgressPercentage { get; set; }
		public double CompletedPercentage { get; set; }
		public double OnHoldPercentage { get; set; }
	}
}
