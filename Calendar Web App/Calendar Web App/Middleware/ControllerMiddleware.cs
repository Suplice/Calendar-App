namespace Calendar_Web_App.Middleware
{
	public class ControllerMiddleware
	{

		private readonly RequestDelegate _next;
		private readonly ILogger<ControllerMiddleware> _logger;

		public AccountControllerMiddleware(RequestDelegate next, ILogger<ControllerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}


		public async Task Invoke(HttpContext context)
		{
			_logger.LogInformation("Request: {RequestMethod} {RequestPath}", context.Request.Method, context.Request.Path);


			if (context.Request.Method == "POST" || context.Request.Method == "PUT")
			{
				context.Request.EnableBuffering();
				var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
				context.Request.Body.Position = 0;
				_logger.LogInformation("Request body: {body}", body);
			};

			await _next(context);

			_logger.LogInformation("Response: {contextResponse}", context.Response.StatusCode);
		}
	}
}
