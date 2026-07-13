namespace PromptlyNote.Core.Exceptions
{
    public class ConflictException : ApiException
    {
        public override int StatusCode => (int)System.Net.HttpStatusCode.Conflict;

        public ConflictException(string message = "A conflict occurred.")
            : base(message)
        {
        }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
