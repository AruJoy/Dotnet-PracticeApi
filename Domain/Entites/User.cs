namespace PracticeApi.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Level { get; private set; }
        // dotnet ORM 인 EF 가 엔티티에 접근하기 위해 선언
        private User() { } //EF 용
        private User(string name, int level)
        {
            Name = name;
            Level = level;
        }
        // builder 패턴: 기존의 readonly, instance 패턴에서 변형
        // 장점:
        //  - 필수값(name)과 선택값(level)을 명시적으로 설정 가능
        //  - 불변(immutable) 엔티티 생성 보장
        //  - 생성자 인자 순서 오류 방지 (가독성 향상)
        //  - 유효성 검사(Validation) 로직을 Build() 내부에 집중시킬 수 있음
        public class Builder
        {
            private string _name = string.Empty;
            private int _level = 1;

            public Builder SetName(string name)
            {
                _name = name;
                return this;
            }
            public Builder SetLevel(int level)
            {
                _level = level;
                return this;
            }

            public User Build()
            {
                if (string.IsNullOrWhiteSpace(_name))
                    throw new InvalidDataException("경고: 이름은 공백으로 할 수 없습니다.");
                return new User(_name, _level);
            }

        }
        public void LevelUp()
        {
            Level++;
        }

    }
}