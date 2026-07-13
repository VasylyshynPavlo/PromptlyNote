using PromptlyNote.Core.Constants;

namespace PromptlyNote.Core.Exceptions
{
    public class NotFoundException : ApiException
    {
        public string Resource { get; }

        public override int StatusCode => (int)System.Net.HttpStatusCode.NotFound;

        public NotFoundException(string resource)
            : base(ExceptionMessages.NotFound(resource))
        {
            Resource = resource;
        }

        public NotFoundException(string resource, Exception innerException)
            : base(ExceptionMessages.NotFound(resource), innerException)
        {
            Resource = resource;
        }
    }
}