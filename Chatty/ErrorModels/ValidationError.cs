namespace ECommerce.ErrorModels
{
    public class ValidationError
    {
        public string Key { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; }
    }
}
