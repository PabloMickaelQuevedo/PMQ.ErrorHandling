namespace PMQ.ErrorHandling.Models
{
    public class ValidationError
    {
        public string Message { get; set; } = default!;
        public string? Field { get; set; }
        public string? Code { get; set; }

        public ValidationError(
            string message,
            string? field = null,
            string? code = null)
        {
            Message = message;
            Field = field;
            Code = code;
        }
    }
}
