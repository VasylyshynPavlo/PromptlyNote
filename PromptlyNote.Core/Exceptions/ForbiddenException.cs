namespace PromptlyNote.Core.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public override int StatusCode => (int)System.Net.HttpStatusCode.Forbidden;

        public ForbiddenException(string message = "Access denied.")
            : base(message)
        {
        }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
