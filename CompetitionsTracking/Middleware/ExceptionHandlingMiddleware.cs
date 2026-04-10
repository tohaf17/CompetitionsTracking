using System.Net;
using System.Text.Json;
using CompetitionsTracking.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CompetitionsTracking.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResult
            {
                Success = false,
                Message = exception.Message
            };

            switch (exception)
            {
                case ValidationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = "VALIDATION_ERROR";
                    errorResponse.Message = "One or more validation errors occurred.";
                    errorResponse.Errors = ex.Errors.Select(e => new ErrorDetail
                    {
                        Field = e.PropertyName,
                        Message = e.ErrorMessage
                    }).ToList();
                    break;

                case BaseException ex:
                    response.StatusCode = (int)ex.StatusCode;
                    errorResponse.ErrorCode = ex.ErrorCode;
                    break;

                default:
                    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                    errorResponse.Message = "An unexpected error occurred on the server.";
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }

    public class ErrorResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public List<ErrorDetail> Errors { get; set; }
    }

    public class ErrorDetail
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
