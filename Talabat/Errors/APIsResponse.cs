
namespace Talabat.APIs.Errors
{
    public class APIsResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public APIsResponse(int statusCode, string? message = null) {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageFromStatusCode(statusCode);
        }

        private string? GetDefaultMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "The request could not be understood or was missing required parameters.",
                401 => "You are not authorized to access this resource. Please log in",
                404 => "The requested resource was not found",
                500 => "An unexpected error occurred. Please try again later"
            };
        }
    }
}
