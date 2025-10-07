// using PracticeApi.Domain.Entities;
// using PracticeApi.Domain.Interfaces;

// namespace PracticeApi.Infrastructure.Repositories
// {
//     public class InMemoryUserRepository : IUserRepository
//     {
//         // 일단 가상의 유저 객체(entity) 생성한다.(서비스 구현이 안됐으므로)
//         public readonly List<User> _users = new()
//         {
//             new User.Builder().SetName("Alice").SetLevel(3).Build(),
//             new User.Builder().SetName("Arujoy").SetLevel(5).Build()
//         };

//         // 현재 entity list 에서 id 가 잂치하는 유저 반환
//         public Task<User?> GetByIdAsync(int id)
//         {
//             // id가 유효하고, user 수가 아이디보다 큰지(enum 되고 전체 목록을 가지고 있으니 자동 증가 인덱스는 id 보다 커야 유효함)
//             // 전체를 불러오고 in memory 작업 하므로 리소스 부하(동적 쿼리 대체 필)
//             // ++
//             // id를 인덱스로 직접 접근하는 단순 구현 (InMemory 전용)
//             // 실제 DB 환경에서는 PK 기반 쿼리로 대체됨 (id 매칭 SELECT)
//             var user = id > 0 && id <= _users.Count ? _users[id - 1] : null;
//             return Task.FromResult(user);
//         }
//         // 모든 유저를 불러온다.
//         // 서비스 규모가 크다면 위험
//         public Task<IEnumerable<User>> GetAllAsync()
//         {
//             return Task.FromResult<IEnumerable<User>>(_users);
//         }
//         // 새로운 User 엔티티를 Builder로 생성 후 인메모리 컬렉션에 추가
//         // 실제 환경에서는 EF Core AddAsync()로 대체되어 DB Insert 수행
//         public Task<User> AddAsync(User user)
//         {
//             // Builder로 복제 생성
//             var newUser = new User.Builder()
//                 .SetName(user.Name)
//                 .SetLevel(user.Level)
//                 .Build();
//             newUser.GetType().GetProperty("Id")?.SetValue(newUser, _users.Count + 1);
//             _users.Add(newUser);
//             // db 응답으로 얻어진 유저 반황
//             return Task.FromResult(newUser);
//         }
//         // InMemory 저장소이므로 별도의 변경사항 커밋 불필요
//         // 실제 EF Core 구현에서는 DB에 트랜잭션 커밋 수행
//         public Task SaveChangesAsync() => Task.CompletedTask;
//     }
// }