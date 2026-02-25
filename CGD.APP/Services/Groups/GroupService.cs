using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;

namespace CGD.APP.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IUserRepository _userRepository;

        public GroupService(IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IUserRepository userRepository)
        {
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _userRepository = userRepository;
        }


        public async Task<GroupDto> GetByIdAsync(Guid id, Guid userId)
        {
            var group = await _groupRepository.GetByIdAsync(id, userId);
            if (group == null) return null;
            return MapToDto(group);
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync(Guid userId)
        {

            var groups = await _groupRepository.GetAllAsync(userId);
            return groups.Select(MapToDto);
        }

        public async Task<GroupDto> CreateAsync(GroupCreateDto dto, Guid userId)
        {
            var group = new Group { Name = dto.Name, Members = new List<GroupMember>(), CreatedBy = userId };
            await _groupRepository.AddAsync(group);
            return MapToDto(group);
        }


        public async Task<GroupDto> UpdateAsync(Guid id, string name, Guid userId)
        {
            var group = await _groupRepository.GetByIdAsync(id, userId);
            if (group == null) return null;
            group.Name = name;
            await _groupRepository.UpdateAsync(group);
            return MapToDto(group);
        }


        public async Task DeleteAsync(Guid id)
        {
            await _groupRepository.DeleteAsync(id);
        }


        public async Task AddUserToGroupAsync(Guid groupId, Guid groupAdmin, Guid userToBeAdd)
        {
            var group = await _groupRepository.GetByIdAsync(groupId, groupAdmin);
            var user = await _userRepository.GetByIdAsync(userToBeAdd);
            if (group == null || user == null) throw new Exception("Group or User not found");
            var member = new GroupMember { GroupId = groupId, UserId = userToBeAdd };
            await _groupMemberRepository.AddAsync(member);
        }

        public async Task RemoveUserFromGroupAsync(Guid groupId, Guid userId)
        {
            await _groupMemberRepository.RemoveAsync(groupId, userId);
        }

        public async Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(Guid userId)
        {
            var memberships = await _groupMemberRepository.GetByUserIdAsync(userId);
            var groupIds = memberships.Select(m => m.GroupId);
            var groups = await _groupRepository.GetAllAsync(userId);
            return groups.Where(g => groupIds.Contains(g.Id)).Select(MapToDto);
        }

        private GroupDto MapToDto(Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Members = group.Members?.Select(static m => new GroupMemberDto
                {
                    GroupId = m.GroupId,
                    UserId = m.UserId,
                    UserName = m.User?.Name ?? "Unknown"
                }).ToList() ?? new List<GroupMemberDto>()
            };
        }
    }
}
