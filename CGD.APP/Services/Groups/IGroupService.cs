using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;

namespace CGD.APP.Services.Groups
{
    public interface IGroupService
    {
        Task<GroupDto> GetByIdAsync(Guid id, Guid userId);
        Task<IEnumerable<GroupDto>> GetAllAsync(Guid userId);
        Task<GroupDto> CreateAsync(GroupCreateDto dto, Guid userId);
        Task<GroupDto> UpdateAsync(Guid id, string name, Guid userId);
        Task DeleteAsync(Guid id);
        Task AddUserToGroupAsync(Guid groupId, Guid groupAdmin, Guid userToBeAdd);
        Task RemoveUserFromGroupAsync(Guid groupId, Guid userId);
        Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(Guid userId);
    }
}
