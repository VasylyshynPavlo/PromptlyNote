namespace PromptlyNote.Core.Exceptions
{
    public class BadRequestException : ApiException
    {
        public override int StatusCode => (int)System.Net.HttpStatusCode.BadRequest;

        public BadRequestException(string message = "Invalid request.")
            : base(message) { }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}