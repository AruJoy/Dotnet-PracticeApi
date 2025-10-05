// dto와 entity간 자동 변환 지원 툴
using AutoMapper;
// 대상 dto namespace
using PracticeApi.Application.DTOs;
// 대상 entity namespace
using PracticeApi.Domain.Entities;

namespace PracticeApi.Application.Common.Mapping
{
    // Profile내부의 매핑 규칙에 의거해 UserProfile 에서 user도메인의 mapping 진행
    public class UserProfile : Profile
    {
        // 기본 생성자
        public UserProfile()
        {
            // 매핑1. User entity를 UserResponse Dto로 변환
            // 각각의 내부 변수간 이름이 같은 지역변수를 자동으로 Mapping
            CreateMap<User, UserResponse>();

            // DTO → Entity 예
            // CreateMap<CreateUserRequest, User>();
        }
    }
}