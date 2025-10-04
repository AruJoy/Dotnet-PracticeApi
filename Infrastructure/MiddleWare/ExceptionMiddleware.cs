using System.Net;
using System.Text.Json;
using PracticeApi.Application.Common.Response;

namespace PracticeApi.Infrastructure.Middleware
{
    // exception 관리 tool 역활을 하는 중간자
    public class ExceptionMiddleware
    {
        //RequestDelegate 란?
        private readonly RequestDelegate _next;
        //ILogger?
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // HttpContext상에 exception이 있는지 확인
            //잘못된 endpoint나, 잘못된 메서드 호출에 반응
            try
            {
                //없다면, 다음 미들웨어 실행(현재는 다음: endpoint 접근)
                await _next(context); // 다음 미들웨어 실행
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = ApiResponse<string>.Fail(ex.Message);
            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}