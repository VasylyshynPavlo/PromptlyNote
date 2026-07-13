namespace PromptlyNote.Core.Exceptions
{
    public class InternalException : ApiException
    {
        public override int StatusCode => (int)System.Net.HttpStatusCode.InternalServerError;

        public InternalException(string message = "An internal error occurred.")
            : base(message)
        {
        }

        public InternalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}