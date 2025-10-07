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
// AutoMapper 라이브러리 사용
using AutoMapper;
// 작성한 Mapper를 사용
using PracticeApi.Application.Common.Mapping;
// EFcore 사용
using Microsoft.EntityFrameworkCore;
using PracticeApi.Infrastructure.Data;

// 
using PracticeApi.Application.validators;

var builder = WebApplication.CreateBuilder(args);

// =====================
// 1️⃣ 서비스 등록 (의존성 주입)
// =====================

// Controller 기반 MVC 서비스 추가
builder.Services.AddControllers();

// Controller 실행 전, FluentValidation을 통해 request 자동 검사
builder.Services.AddFluentValidationAutoValidation();
// CreateUserRequest를 사용하는 endpoint에서 자동 검사 진행
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
//builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()) 그냥쓴다면
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
// Di 생명주기 명시 - AddScoped: 해당 DI를  scoped 생명주기로 등록
// 매 HTTP 요청마다 독립적인 생성과 작동
// builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

// 작성한 db context를 등록
// 등록만 된 뒤, 각 HTTP요청에서 사용시 생성 => scoped di주기
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
// Repository를 EFCore 기반 구현체로 교체
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
// Application Service 등록
builder.Services.AddScoped<UserAppService>();
// UserProfile을 포함하는 어셈블리를 스캔해서,
// AutoMapper 매핑 설정을 전부 컨테이너에 자동으로 등록
builder.Services.AddAutoMapper(typeof(UserProfile));

// 모든 요청이 공통적으로 동작해야 하는 경우, 아래방식으로 singleton 삽입
// builder.Services.AddSingleton<>

// 매 요청시 새로운 객체로 진행해야 하면, 아래 Transient 삽입
// builder.Services.AddTransient<>

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
