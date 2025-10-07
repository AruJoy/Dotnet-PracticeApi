using AutoMapper;
using PracticeApi.Application.DTOs;
using PracticeApi.Domain.Interfaces;
// 정규식을 위한 import
using System.Text.RegularExpressions;

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
        private readonly ILogger<UserAppService> _logger;
        // mapper의 경우, 비즈니스 로직흐름에 영향을 주지 않고 공통적으로 사용
        // -> singleton

        // 검색을 위한 정규식
        readonly Regex regex = new Regex(@"^[a-zA-Z가-힣]+$");

        private readonly IMapper _mapper;
        // service 계층, 레포지토리를 참조해서 사용
        // interFace 를 받아서 사용(특정구현체에 제한 X)
        // IUserRepository 인터페이스를 주입받음
        // → DIP(의존성 역전 원칙) 준수: 구체 구현(InMemory, EFCore 등)과 분리
        // → OCP(개방-폐쇄 원칙): 새로운 Repository 구현이 추가돼도 서비스 코드는 수정 불필요
        // 기본 생성자의 매개변수에 DI 명시하고, class 냐부 변수에 할당
        public UserAppService(IUserRepository repo,
                                ILogger<UserAppService>
                                logger, IMapper mapper)
        {
            _repo = repo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            // interface 의 해당 함수가 async 이므로,
            // 상위에서는 결과 보장되어야 하면 await 키워드 필수
            // return await _repo.GetAllAsync();

            // _repo 에서 반환한 User entity 를 바로 사용하지 않음
            // 왜? 사용되는 service 마다 필요한 값과 엔티티에게 요구하는 행동이 다르기 때문
            _logger.LogInformation("모든 유저 조회.");
            var users = await _repo.GetAllAsync();

            // js 의 Map 과 같은 동작을 하는 메서드인듯?
            // return users.Select(u => new UserResponse
            // {
            //     Id = u.Id,
            //     Name = u.Name,
            //     Level = u.Level
            // });

            // 작성한 mapper 사용
            // users entity list 를 UserResponse dto로 변환 반환
            // 여기서 Map은 iterator 연산이 아닌, Mapper의 함수
            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            // return await _repo.GetByIdAsync(id);
            _logger.LogInformation("유저 ID {Id} 조회 요청", id);
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;

            // return new UserResponse
            // {
            //     Id = user.Id,
            //     Name = user.Name,
            //     Level = user.Level
            // };

            return _mapper.Map<UserResponse>(user);
        }
        // 새로운 유저 엔티티를 Builder 패턴으로 생성
        // - 유효성 검사(이름 공백 등)는 도메인 내부에서 수행됨
        // - Application 계층은 도메인 객체 조립만 담당
        // - Repository를 통해 저장, SaveChangesAsync로 트랜잭션 커밋

        public async Task<UserResponse> CreateAsync(string name, int level = 1)
        {
            var user = new Domain.Entities.User.Builder()
                .SetName(name)
                .SetLevel(level)
                .Build();
            // 추적 상태 등록
            await _repo.AddAsync(user);
            // DB에 반영
            await _repo.SaveChangesAsync();
            // return new UserResponse
            // {
            //     // Create 시에 builder 에서 설정이 안됐는데? => 이후 id 를 쓰는 로직 사용시 0의 아이디 참조 가능성?
            //     // dotnet orm 에서 SaveChangesAsync 하며 할당?
            //     Id = user.Id,
            //     Name = user.Name,
            //     Level = user.Level
            // };
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<IEnumerable<UserResponse>> SearchAsync(string? keyword, int? minLevel, int? maxLevel)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // 유저 이름 인자에 대한 정규식 검사
                if (!regex.IsMatch(keyword))
                    // 정규식과 다를경우,Exception throw
                    throw new ArgumentException("영어, 한글 외 숫자와 특수문자는 입력할 수 없습니다.");
            }
            if (minLevel.HasValue)
            {
                if (minLevel.Value < 1)
                    throw new ArgumentException("레벨은 최소 1 이상 입력해 주세요.");
            }
            if (maxLevel.HasValue)
            {
                if (maxLevel.Value > 99)
                    throw new ArgumentException("레벨은 최대 99 이하로 입력해 주세요.");
            }
            // 새로 생성한 repository 함수 호출
            var users = await _repo.SearchAsync(keyword, minLevel, maxLevel);
            // auto mapper 를 톨해 Response 반환
            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }
    }
}