using PracticeApi.Domain.Entities;

namespace PracticeApi.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
    }
}