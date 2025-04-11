namespace Talabat.APIs.Errors
{
    public class APIsExceptionError : APIsResponse
    {
        public string? Details { get; set; }
        public APIsExceptionError(string? message = null, string? details = null):base(500,message)
        {
            Details = details;
        }
    }
}
