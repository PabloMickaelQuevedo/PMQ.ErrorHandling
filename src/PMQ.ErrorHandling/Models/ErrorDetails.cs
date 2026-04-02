using Microsoft.AspNetCore.Mvc;

namespace PMQ.ErrorHandling.Models
{
    public class ErrorDetails : ProblemDetails
    {
        public IEnumerable<ValidationError>? Errors { get; set; }
        public string? TraceId { get; set; }
    }
}
