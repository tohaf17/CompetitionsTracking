using System.Net;

namespace CompetitionsTracking.Domain.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }
    }
}
