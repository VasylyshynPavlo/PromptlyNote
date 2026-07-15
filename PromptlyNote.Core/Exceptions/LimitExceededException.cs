namespace PromptlyNote.Core.Exceptions
{
    public class LimitExceededException : ApiException
    {
        public override int StatusCode => (int)System.Net.HttpStatusCode.Conflict;

        public LimitExceededException(string message = "The limit for this resource has been exceeded.")
            : base(message)
        {
        }

        public LimitExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
