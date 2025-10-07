// dotnet orm Efcore 사용
using Microsoft.EntityFrameworkCore;
// 이 레포지토리에서 사용할 도메인 엔티티 import
using PracticeApi.Domain.Entities;
// 해당 클래스에서 구현할 interface
using PracticeApi.Domain.Interfaces;
// 설정한 Db context
using PracticeApi.Infrastructure.Data;

namespace PracticeApi.Infrastructure.Repositories
{
    // interface를 상속받아 선언
    public class EfUserRepository : IUserRepository
    {
        //di 주입 필드
        private readonly AppDbContext _context;

        //di 자동 주입을 위한 생성자
        public EfUserRepository(AppDbContext context)
        {
            _context = context;
        }
        // async -> 외부 db 에서 받아오기 때문에 실행후 응답간 gap 존재
        // 때문에 동기처리를 위해 async 키워드 사용
        // 후시(응답 이후)에 반환될 User general 사용
        public async Task<User?> GetByIdAsync(int id)
        {
            // ef 코어의 DbContext에서 column id 일치하는 row 반환
            return await _context.Users.FindAsync(id);
        }
        // user목록을 IEnumerable 로 enum처리해서 반환( 호출 후 in memory에서 다룸)
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // 여러 row를 불러오기 때문에, ToListAsync 사용
            // DB에서 모든 유저를 조회하여 메모리 컬렉션(List<User>)로 반환
            return await _context.Users.ToListAsync();
        }
        // user 등록
        public async Task<User> AddAsync(User user)
        {
            // repository context 에 해당 유저 commit
            _context.Users.Add(user);
            // 처리 완료와 동시에 등록이 되어야 하므로
            // 영속화 SaveChangesAsync 호출
            await _context.SaveChangesAsync();
            return user;
        }

        // 현재 repository 상에 commit된 추적사항들을 db에 반영
        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        // 유저 조회를 위한 동적 쿼리 함수
        public async Task<IEnumerable<User>> SearchAsync(string? keyword, int? minLevel, int? maxLevel)
        {
            // User domain 에 대한 쿼리 생성
            // IQueryable: 매 조회마다 새로운 쿼리 생성 -> 생명주기 == transient
            IQueryable<User> query = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Name column 의 Where문 구문
                query = query.Where(u => u.Name.Contains(keyword));
            }
            if (minLevel.HasValue)
            {
                query = query.Where(u => u.Level >= minLevel.Value);
            }
            if (maxLevel.HasValue)
            {
                query = query.Where(u => u.Level <= maxLevel.Value);
            }

            // 쿼리중 변경 감지 비활성화: AsNoTracking
            // 읽기 전용 성능 향상 솔루션
            return await query.AsNoTracking()
                                // level에 따라 내림차순
                                .OrderByDescending(u => u.Level)
                                // 해당하는 유저를 DB에서 조회하여 메모리 컬렉션(List<User>)로 반환
                                .ToListAsync();
        }
    }
}