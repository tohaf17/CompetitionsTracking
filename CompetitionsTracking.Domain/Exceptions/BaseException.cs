using System;
using System.Net;

namespace CompetitionsTracking.Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        protected BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = "INTERNAL_SERVER_ERROR")
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
