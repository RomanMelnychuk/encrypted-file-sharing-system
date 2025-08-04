using AutoMapper.Internal;
using FileGuard.API.Models;
using FileGuard.Shared;
using FileGuard.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FileGuard.API.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var response = new ErrorModel();

        if (exception is StatusCodeBaseException statusCodeException)
        {
            response.ErrorCode = statusCodeException.ErrorCode;
            response.Message = statusCodeException.Message;
            response.Status = statusCodeException.StatusCode;

            httpContext.Response.StatusCode = response.Status;

            await httpContext.Response.WriteAsJsonAsync(response, CancellationToken.None);
            return true;
        }

        response = new ErrorModel(
            StatusCodes.Status500InternalServerError, "Server error", ErrorCodes.InternalServerError);

        httpContext.Response.StatusCode = response.Status;

        await httpContext.Response.WriteAsJsonAsync(response, CancellationToken.None);

        return true;
    }
}
