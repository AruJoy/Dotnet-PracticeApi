using System.Net;
using System.Text.Json;
using PracticeApi.Application.Common.Response;

namespace PracticeApi.Infrastructure.Middleware
{
    // exception 관리 tool 역활을 하는 중간자
    // ASP.NET Core 파이프라인에서 발생하는 모든 예외를
    // 한 곳에서 처리해주는 “전역 예외 처리기(Global Exception Handler)”
    public class ExceptionMiddleware
    {
        //RequestDelegate 란?
        // ASP.NET Core 미들웨어는 체인 구조로 연결되어 있으며,
        // 각 미들웨어는 다음 미들웨어를 가리키는 delegate(_next)를 받는다.
        // => next(context)를 호출하면 다음 미들웨어로 요청이 넘어감
        private readonly RequestDelegate _next;
        //ILogger?
        // ASP.NET Core 내장 로깅 시스템 (DI로 자동 주입됨)
        // - LogError, LogInformation, LogWarning 등 레벨별 로그 제공
        // - Serilog, Seq, Console 등으로 출력 가능
        private readonly ILogger<ExceptionMiddleware> _logger;
        // 생성자: DI 컨테이너에서 next/logger를 자동 주입받음
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        // - context : 현재 요청/응답의 모든 정보(HttpRequest, HttpResponse, User 등)
        // - try-catch를 통해 아래 미들웨어(컨트롤러 포함)에서 터진 모든 예외를 잡는다.
        public async Task InvokeAsync(HttpContext context)
        {
            // 요청 처리중에 exception이 있는지 확인
            // Controller 실행 중 예외 (비즈니스 로직, 데이터 접근 오류 등)에 반응
            try
            {
                // 미들웨어는 계층 구조로, 다음 미들웨어는 하위로 취급하여 처리됨
                // [ExceptionMiddleware]
                //     └── [LoggingMiddleware]
                //             └── [RoutingMiddleware]
                //                 └── [Controller 실행]

                //없다면, 다음 미들웨어 실행(현재는 다음: endpoint 접근)
                await _next(context); //  요청 처리 중 (_next 실행 구간) 예외가 발생하는지를 감시
            }
            catch (Exception ex)
            {
                // error 로깅
                _logger.LogError(ex, "Unhandled exception occurred.");
                // exception handler를 통해 아까 작성한 전역 예외처리 반환
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // 응답은 json을 통해 응답
            context.Response.ContentType = "application/json";
            // 내부 서비스 오류(500) code 포함
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // switch (ex)
            //     {
            //         case ArgumentException:
            //             context.Response.StatusCode = 400;
            //             break;
            //         case UnauthorizedAccessException:
            //             context.Response.StatusCode = 401;
            //             break;
            //         case KeyNotFoundException:
            //             context.Response.StatusCode = 404;
            //             break;
            //         default:
            //             context.Response.StatusCode = 500;
            //             break;
            //     }
            // 등으로 세분화

            // 전역 예외처리 Fail 에 상세 이유를 포함해서 생성
            var response = ApiResponse<string>.Fail(ex.Message);
            // json 직렬화 툴을 이용해 json 응답 형식으로 변환
            // System.Text.Json을 이용해 ApiResponse 객체를 JSON 문자열로 직렬화
            // .NET 내장 고성능 직렬화가 Newtonsoft.Json보다 경량
            var json = JsonSerializer.Serialize(response);
            // 직렬화된 JSON 문자열을 HttpResponse.Body 스트림에 기록하여 클라이언트에 응답 반환
            return context.Response.WriteAsync(json);
        }
    }
}