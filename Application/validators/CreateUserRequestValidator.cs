// dotnet dto validator tool 사용
using FluentValidation;
// 해당 validator 에서 검사할 namespace 불러오기
using PracticeApi.Application.DTOs;

namespace PracticeApi.Application.Validators
{
    // FluentValidation 에서 지원하는 추상validator에 검사하려는 dto를 제네릭으로 넣어서 class 생성
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                // Cascade: 여러 문제가 있어도, 마주하는 첫번쩨 예외만 처리
                .Cascade(CascadeMode.Stop)
                // 공백 불가 룰 추가
                .NotEmpty().WithMessage("이름은 공백으로 둘 수 없습니다.")
                // 영어 소문자/대문자만 가능
                .Matches(@"\A\S+\z").WithMessage("이름엔 숫자와 특수문자를 포함할 수 없습니다")
                // (한글, _) 정규식은?
                //.Matches(@"^[a-zA-Z가-힣]+$").WithMessage("이름에는 영문 또는 한글만 사용할 수 있습니다.")

                // 최소 최대 룰 추가
                .MinimumLength(2).WithMessage("이름은 촤소 2자 이상이여야 합니다.")
                .MaximumLength(20).WithMessage("이름은 최대 20자 까지 가능합니다.");

            RuleFor(x => x.Level)
                // level은 int형이라 그냥 하나씩 넓히고 <, > 연산으로 대체 
                .GreaterThan(0).WithMessage("최소레벨은 1 입니다.")
                .LessThan(100).WithMessage("최대 레벨은 99 입니다.");
        }
    }
}