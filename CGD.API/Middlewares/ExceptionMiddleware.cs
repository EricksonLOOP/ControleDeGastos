using CGD.CrossCutting.Exceptions;

namespace ControleDeGastos.Middlewares;

using System.Net;
using System.Text.Json;
using CGD.CrossCutting.Exceptions;


public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            UserNotFoundException => HttpStatusCode.NotFound,
            ExpenseNotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            CategoryNotFoundException => HttpStatusCode.NotFound,
            InvalidCategoryPurposeException => HttpStatusCode.BadRequest,
            InvalidTransactionForMinorException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            GroupNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = ex.Message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
}
