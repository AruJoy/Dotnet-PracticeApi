// ASP.NET Core MVC의 API 전용 기능 제공 (ControllerBase 상속용)
// RESTful 엔드포인트 구현을 위한 Attribute 기반 라우팅 및 응답 처리 지원
using Microsoft.AspNetCore.Mvc;
// service 계층
using PracticeApi.Application.Services;
// using PracticeApi.Domain.Entities;
using PracticeApi.Application.DTOs;
using PracticeApi.Application.Common.Response;
// 컨트롤러 관리 네임스페이스
// 도메인들의 엔드포인트를 총합하여 관리
using PracticeApi.Application.Common.Attributes;

namespace PracticeApi.Controllers
{
    // 해당 클래스가 controller 관리 하기위한 class 임을 프레임 워크에 알림
    // 이하의 각 메서드 함수를 엔드포인트에 등록
    [ApiController]
    // 해당 URL의 최상위 URI
    // 추가 URL가 필요할경우, 각 함수마다 명시
    [Route("api/[controller]")]
    // ControllerBase == mvc core 의 controller 전용 abstract class
    public class UserController(UserAppService service) : ControllerBase
    {
        // service 레이어 참조
        // required 한정자 사용해야하나? -> direct injection 하기때문에 불필요
        private readonly UserAppService _service = service;

        [ApiProducesResponse(typeof(UserResponse))]
        // get 메서드
        [HttpGet]
        // IActionResult: 애당 컨트롤러의 결과를 내부의 ActionContext 를 통해 반환
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            // status : 200, code 등을 포함해,
            // data: users(json list 번환) 하여 반환
            return Ok(users);
        }
        [ApiProducesResponse(typeof(UserResponse))]
        // get 메서드
        // url: api/[controller]/id
        // id 를 param으로 받음
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                // 찾는 유저가 없을경우, resource 부재 404번 status 를 포함한 정규 response 반환
                return NotFound(ApiResponse<string>.Fail("user not found"));
            // 찾으면 200 ok
            return Ok(user);
        }

        [ApiProducesResponse(typeof(UserResponse))]
        // db에 Create 하는 메소드
        // 민감정보등의 보호가 필요함으로 header 가 아닌, 최소한의 보호를 위해 패킷의 body 를 통해 전달
        // 이또한 패킷 탈취후, 패킷 분석등을 통해 탈취 가능하므로 https를 통한 인증서 암호화 필요
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var user = await _service.CreateAsync(request.Name, request.Level);
            // 등록후 create 가 제대로 되었는지 확실히 하기위해 db 에서 다시 반환
            return CreatedAtAction(nameof(GetById),
                new { id = user.Id },
                ApiResponse<UserResponse>.Ok(user, "User created successfully"));
        }
        [ApiProducesResponse(typeof(UserResponse))]
        [HttpGet("search")]
        // ?쿼리명 = 값
        // 형태의 쿼리인자: [FromQuery] 어트리뷰션 필요
        public async Task<IActionResult> Search([FromQuery] UserSearchRequest request)
        {
            int page = request.Page ?? 1;
            int pageSize = request.PageSize ?? 10;

            var result = await _service.SearchAsync(
                request.Keyword, request.MinLevel, request.MaxLevel, page, pageSize);

            return Ok(result);

        }
    }
}