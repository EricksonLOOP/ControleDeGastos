using CGD.Domain.Entities;

namespace CGD.Domain.IRepositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<IReadOnlyList<User>> GetPagedAsync(int page, int pageSize);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<List<User>> GetEnrichedUsers(Guid adminGroupId);
}