using Microsoft.OpenApi.Models;
using PracticeApi.Infrastructure.Middleware;
using PracticeApi.Application.services;
using PracticeApi.Domain.Interfaces;
using PracticeApi.Infrastructure.Repositories;
// dotnet validator tool FluentValidation 사용 
using FluentValidation;
// ASP.NET Core: .NET 기반 프레임 워크
using FluentValidation.AspNetCore;
using PracticeApi.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// =====================
// 1️⃣ 서비스 등록 (의존성 주입)
// =====================

// Controller 기반 MVC 서비스 추가
builder.Services.AddControllers();

// Controller 실행 전, FluentValidation을 통해 request 자동 검사
builder.Services.AddFluentValidationAutoValidation();
// CreateUserRequest를 사용하는 endpoint에서 자동 검사 진행
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequest>();
//builder.Services.AddValidatorsFromAssemblyContaining() 그냥쓴다면
// AbstractValidator 를 상속받은 동일 Assembly 모든 validator 를 자동 등록

// using PracticeApi.Application.validators 후에,
// builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
// 식으로 validator 클래스 단위로 등록하는게 표준

// Swagger/OpenAPI 설정
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Practice API",
        Version = "v1",
        Description = "A layered architecture practice API (Domain / Application / Infrastructure)"
    });
    c.TagActionsBy(api => new[] { api.GroupName ?? "Default" });
});

// Repository 인터페이스 - 구현체 연결 (DI 등록)
// IUserRepository 의존 시 InMemoryUserRepository 주입
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

// Application Service 등록
builder.Services.AddScoped<UserAppService>();

// =====================
// 2️⃣ 앱 빌드
// =====================
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

// =====================
// 3️⃣ Swagger 설정
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Practice API v1");
        c.RoutePrefix = string.Empty; // root URL(/)에서 Swagger 열기 (선택)
    });
}

// =====================
// 4️⃣ 미들웨어 파이프라인
// =====================
app.UseHttpsRedirection();

// Controller 라우팅 매핑
app.MapControllers();

// 최종 미들웨어 파이프 라인
// app.UseMiddleware<ExceptionMiddleware>(); // 1. 가장 먼저 예외 잡기
// app.UseHttpsRedirection();               // 2. HTTP→HTTPS
// app.UseRouting();                        // 3. 라우팅 테이블 준비
// app.UseAuthentication();                 // 4. 인증
// app.UseAuthorization();                  // 5. 권한
// app.MapControllers();                    // 6. 엔드포인트 (여기서 실제 컨트롤러 실행)
// =====================
// 5️⃣ 앱 실행
// =====================
app.Run();
