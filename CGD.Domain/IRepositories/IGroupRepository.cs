using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.Domain.Entities;

namespace CGD.Domain.IRepositories
{
    public interface IGroupRepository
    {
        Task<Group> GetByIdAsync(Guid id,  Guid userId);
        Task<IEnumerable<Group>> GetAllAsync(Guid userId);
        Task AddAsync(Group group);
        Task UpdateAsync(Group group);
        Task DeleteAsync(Guid id);
    }
}
