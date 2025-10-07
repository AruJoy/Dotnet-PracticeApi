// dotnet dto validator tool 사용
using FluentValidation;
// 해당 validator 에서 검사할 namespace 불러오기
using PracticeApi.Application.DTOs;

namespace PracticeApi.Application.Validators
{
    // FluentValidation 에서 지원하는 추상validator에 검사하려는 dto를 제네릭으로 넣어서 class 생성
    public class SearchUserRequestValidator : AbstractValidator<UserSearchRequest>
    {
        public SearchUserRequestValidator()
        {
            RuleFor(x => x.Keyword)
                .Matches(@"^[a-zA-Z가-힣]+$")
                .When(x => !string.IsNullOrWhiteSpace(x.Keyword))
                .WithMessage("영어, 한글 외 숫자와 특수문자는 입력할 수 없습니다.");

            RuleFor(x => x.MinLevel)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(99)
            .When(x => x.MinLevel.HasValue)
            .WithMessage("레벨은 최소 1 이상이어야 합니다.");

            RuleFor(x => x.MaxLevel)
                .LessThanOrEqualTo(99)
                .GreaterThanOrEqualTo(x => x.MinLevel)
                .When(x => x.MaxLevel.HasValue)
                .WithMessage("레벨은 최대 99 이하이어야 합니다.");
        }
    }
}