namespace PracticeApi.Application.DTOs
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
    }

    public class UserSearchRequest
    {
        public string? Keyword { get; set; }
        public int? MinLevel { get; set; }
        public int? MaxLevel { get; set; }
    }
}