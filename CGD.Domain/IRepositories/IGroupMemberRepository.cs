using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.Domain.Entities;

namespace CGD.Domain.IRepositories
{
    public interface IGroupMemberRepository
    {
        Task<IEnumerable<GroupMember>> GetByGroupIdAsync(Guid groupId);
        Task<IEnumerable<GroupMember>> GetByUserIdAsync(Guid userId);
        Task AddAsync(GroupMember groupMember);
        Task RemoveAsync(Guid groupId, Guid userId);
        Task<IReadOnlyList<GroupMember>> GetAllByGroupAdminId(Guid userId);
    }
}
