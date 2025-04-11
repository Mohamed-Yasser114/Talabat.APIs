namespace Talabat.APIs.Errors
{
    public class APIsValidationErrorResponse : APIsResponse
    {
        public List<string> Errors { get; set; }
        public APIsValidationErrorResponse(): base(400)
        {
            Errors = new List<string>();
        }
    }
}
