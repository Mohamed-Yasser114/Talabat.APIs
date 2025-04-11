using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlwares
{
    public class APIsExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<APIsExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public APIsExceptionMiddleware(RequestDelegate next, ILogger<APIsExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            
            try
            {
                await _next.Invoke(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                httpContext.Response.StatusCode = 500;

                httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ? new APIsExceptionError(ex.Message, ex.StackTrace.ToString())
                                                    : new APIsExceptionError();

                var option = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, option);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}
