namespace ECommerce.ErrorModels
{
    public class ValidationErrorToReturn
    {
        public int StatusCode { get; set; } = 400; // Default to Bad Request
        public string Message { get; set; } = "There are some fields has some errors";
        public IEnumerable<ValidationError> Errors { get; set; }
    }
}
