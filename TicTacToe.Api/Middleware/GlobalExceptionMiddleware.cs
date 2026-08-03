using System.Net;
using System.Text.Json;
using TicTacToe.Core.Exceptions;

namespace TicTacToe.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (GameNotFoundException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.NotFound, "Game Not Found", ex.Message);
            }
            catch (InvalidMoveException ex)
            {
                await WriteProblemAsync(context, HttpStatusCode.BadRequest, "Invalid Move", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing {Path}", context.Request.Path);
                await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error",
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
