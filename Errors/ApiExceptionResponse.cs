namespace SocialApp.Errors
{
    public class ApiExceptionResponse
    {
        public ApiExceptionResponse(int StatusCode, string message = null, string traceId = null)
        {
            this.StatusCode = StatusCode;
            Message = message;
            TraceId = traceId;
        }

        public int StatusCode { get; }
        public string Message { get; }
        public string TraceId { get; }
    }
}
