using Newtonsoft.Json;
using System.Net;

namespace TaskManagement.Middlewares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var response = new
			{
				message = "An unexpected error occurred.",
				detail = exception.Message // Include exception message for debugging (optional)
			};

			var jsonResponse = JsonConvert.SerializeObject(response);
			return context.Response.WriteAsync(jsonResponse);
		}
	}
}
