using System.Net;
using ECommerce.SharedKernel.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using FluentValidation;

namespace ECommerce.BuildingBlocks.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                
                // 提取所有验证错误信息
                var errorMessages = ex.Errors.Select(e => e.ErrorMessage).ToList();
                var combinedMessage = string.Join("; ", errorMessages);
                
                await WriteResponse(context, HttpStatusCode.BadRequest, 
                    ApiResponse<string>.Fail("VALIDATION_ERROR", combinedMessage));
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.Unauthorized, ApiResponse<string>.Fail("UNAUTHORIZED", ex.Message));
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "BadRequest: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.BadRequest, ApiResponse<string>.Fail("BAD_REQUEST", ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "BusinessError: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.BadRequest, ApiResponse<string>.Fail("BUSINESS_ERROR", ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await WriteResponse(context, HttpStatusCode.InternalServerError, ApiResponse<string>.Fail("INTERNAL_ERROR", "服务器内部错误"));
            }
        }

        private static async Task WriteResponse<T>(HttpContext context, HttpStatusCode statusCode, ApiResponse<T> body)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(body);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
