using ECommerce.ErrorModels;
using ServiceAbstraction;


namespace ECommerce.CustomMiddlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingService _logger;
        public CustomExceptionMiddleware(ILoggingService logger, RequestDelegate next)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ILoggingService logger)
        {
            try
            {
                await _next.Invoke(context);
                await NotFoundResourceHandler(context, logger);
            }
            catch (Exception ex)
            {
                await ExceptionsHandler(context, ex, logger);
            }
        }

        private async Task ExceptionsHandler(HttpContext context, Exception ex, ILoggingService logger)
        {
            switch (ex)
            {
                case Domain.Exceptions.NotFoundException notFoundException:
                    _logger.LogError(ex.Message, ex);
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case Domain.Exceptions.BadRequestException badRequestException:
                    _logger.LogError(ex.Message, ex);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                default:
                    _logger.LogError(ex.Message, ex);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }
            var response = new ErrorModel()
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task NotFoundResourceHandler(HttpContext context, ILoggingService logger)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                _logger.LogWarning("404 Not Found: {Path}", context.Request.Path);
                var response = new ErrorModel()
                {
                    statusCode = context.Response.StatusCode,
                    message = $"Resource {context.Request.Path} not found."
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
