using PracticeApi.Domain.Entities;
using PracticeApi.Application.Common.Response;
using PracticeApi.Application.DTOs;
namespace PracticeApi.Domain.Interfaces
{
    //repository 관리 interface
    public interface IUserRepository
    {
        //Task<> : 비동기 처리시, 반환값을 미리 알려주는 제너릭
        // Repository 구현체가 DB I/O, 네트워크 호출 등 비동기 작업을 수행할 가능성을 반영
        // id 와 일치하는 유저 반환
        // 없을시 null 반환?
        Task<User?> GetByIdAsync(int id);
        // 모든 user를 enum 처리해서 반환.
        Task<IEnumerable<User>> GetAllAsync();
        // 유저 등록후 완료된 유저 반환
        Task<User> AddAsync(User user);
        // Repository의 변경사항을 DB에 실제 반영
        // EF Core의 ChangeTracker가 추적한 변경 내역을 커밋(Commit)
        // (JPA의 flush와 비슷하지만, EF Core는 SaveChanges() 시점에 바로 반영)EF Core는 SaveChangesAsync() 호출 시점에
        // 내부적으로 트랜잭션을 생성하고, 모든 변경 사항을 한 번에 커밋.
        // 즉, 여러 Add/Update/Delete가 한 번의 SQL 트랜잭션으로 처리.
        Task SaveChangesAsync();
        // 새로운 매소드 Interface에 명시
        Task<IEnumerable<User>> SearchAsync(string? keyword, int? minLevel, int? maxLevel, int page = 1, int pageSize = 10);
        Task<PagedResult<UserResponse>> SearchProjectionAsync(
            string? keyword, int? minLevel, int? maxLevel, int page = 1, int pageSize = 10);
    }
}