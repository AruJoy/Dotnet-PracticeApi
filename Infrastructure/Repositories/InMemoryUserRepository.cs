using PracticeApi.Domain.Entities;
using PracticeApi.Domain.Interfaces;

namespace PracticeApi.Infrastructure.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        public readonly List<User> _users = new()
        {
            new User.Builder().SetName("Alice").SetLevel(3).Build(),
            new User.Builder().SetName("Arujoy").SetLevel(5).Build()
        };

        public Task<User?> GetByIdAsync(int id)
        {
            var user = id > 0 && id <= _users.Count ? _users[id - 1] : null;
            return Task.FromResult(user);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<User>>(_users);
        }

        public Task<User> AddAsync(User user)
        {
            var newUser = new User.Builder()
                .SetName(user.Name)
                .SetLevel(user.Level)
                .Build();

            _users.Add(newUser);
            return Task.FromResult(newUser);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}