// EFcore import
using Microsoft.EntityFrameworkCore;

// db 컨텍스트에서 다룰 Entity들을 import
using PracticeApi.Domain.Entities;

namespace PracticeApi.Infrastructure.Data
{
    // Db관리를 위한 Context class
    // DbContext: EF Core에서 ORM 기능(엔티티 추적, 쿼리 변환, 변경 감지 등)을 제공하는 기본 컨텍스트 클래스
    public class AppDbContext : DbContext
    {
        // 기본 생성자 (옵션명시)
        // 현재:EFcore의 기본설정 사용
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        // Users는 User 엔티티에 대응하는 쿼리 진입점으로, 실제로 DB에 접근할 때 쿼리로 변환되어 실행된다.
        public DbSet<User> Users => Set<User>();

        // db 초기화 메서드
        // User 엔티티의 프로퍼티 정의를 기반으로, 매핑되는 Users 테이블 구조를 생성.
        // 운영 환경에서는 마이그레이션 자동 적용(EnsureCreated, Migrate)은 피해야 한다.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}