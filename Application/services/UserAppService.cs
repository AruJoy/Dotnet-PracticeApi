using PracticeApi.Domain.Entities;
using PracticeApi.Domain.Interfaces;

namespace PracticeApi.Application.services
{
    // Application Layer의 서비스 클래스
    // 비즈니스 유즈케이스를 정의하고, Repository 인터페이스를 이용해 도메인 객체를 관리한다.
    // 도메인(Entity)의 생성/검증 로직을 직접 호출하여 비즈니스 흐름을 조립한다.
    public class UserAppService
    {
        // 초기 생성시 설정 후 임의 조작 제한
        // 캡슐화
        private readonly IUserRepository _repo;

        // service 계층, 레포지토리를 참조해서 사용
        // interFace 를 받아서 사용(특정구현체에 제한 X)
        // IUserRepository 인터페이스를 주입받음
        // → DIP(의존성 역전 원칙) 준수: 구체 구현(InMemory, EFCore 등)과 분리
        // → OCP(개방-폐쇄 원칙): 새로운 Repository 구현이 추가돼도 서비스 코드는 수정 불필요
        public UserAppService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // interface 의 해당 함수가 async 이므로,
            // 상위에서는 결과 보장되어야 하면 await 키워드 필수
            return await _repo.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
        // 새로운 유저 엔티티를 Builder 패턴으로 생성
        // - 유효성 검사(이름 공백 등)는 도메인 내부에서 수행됨
        // - Application 계층은 도메인 객체 조립만 담당
        // - Repository를 통해 저장, SaveChangesAsync로 트랜잭션 커밋

        public async Task<User> CreateAsync(string name, int level = 1)
        {
            var user = new User.Builder()
                .SetName(name)
                .SetLevel(level)
                .Build();
            // 추적 상태 등록
            await _repo.AddAsync(user);
            // DB에 반영
            await _repo.SaveChangesAsync();
            return user;
        }
    }
}