using Microsoft.OpenApi.Models;
using PracticeApi.Infrastructure.Middleware;
using PracticeApi.Application.services;
using PracticeApi.Domain.Interfaces;
using PracticeApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =====================
// 1️⃣ 서비스 등록 (의존성 주입)
// =====================

// Controller 기반 MVC 서비스 추가
builder.Services.AddControllers();

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
