namespace PracticeApi.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }

        private User() { } //EF 용
        private User(string name, int level)
        {
            Name = name;
            Level = level;
        }

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