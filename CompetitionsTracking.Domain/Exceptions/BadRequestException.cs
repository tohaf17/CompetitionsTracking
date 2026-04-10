using System.Net;

namespace CompetitionsTracking.Domain.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) 
            : base(message, HttpStatusCode.BadRequest, "BAD_REQUEST")
        {
        }
    }
}
