using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;
using CGD.CrossCutting.Exceptions;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;

namespace CGD.APP.Services.Groups
{
    public class GroupService(
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IUserRepository userRepository)
        : IGroupService
    {
        public async Task<GroupDto> GetByIdAsync(Guid id, Guid userId)
        {
            var group = await groupRepository.GetByIdAsync(id, userId);
            if (group == null) return null;
            return MapToDto(group);
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync(Guid userId)
        {

            var groups = await groupRepository.GetAllAsync(userId);
            return groups.Select(MapToDto);
        }

        public async Task<GroupDto> CreateAsync(GroupCreateDto dto, Guid userId)
        {
            var group = new Group { Name = dto.Name, Members = new List<GroupMember>(), CreatedBy = userId };
            await groupRepository.AddAsync(group);
            return MapToDto(group);
        }


        public async Task<GroupDto> UpdateAsync(Guid id, string name, Guid userId)
        {
            var group = await groupRepository.GetByIdAsync(id, userId);
            if (group == null) return null;
            group.Name = name;
            await groupRepository.UpdateAsync(group);
            return MapToDto(group);
        }


        public async Task DeleteAsync(Guid id)
        {
            await groupRepository.DeleteAsync(id);
        }


        public async Task AddUserToGroupAsync(Guid groupId, Guid groupAdmin, Guid userToBeAdd)
        {
            var group = await groupRepository.GetByIdAsync(groupId, groupAdmin);
            var user = await userRepository.GetByIdAsync(userToBeAdd);
            if (group == null || user == null) throw new GroupNotFoundException();
            var member = new GroupMember { GroupId = groupId, UserId = userToBeAdd };
            await groupMemberRepository.AddAsync(member);
        }

        public async Task RemoveUserFromGroupAsync(Guid groupId, Guid userId)
        {
            await groupMemberRepository.RemoveAsync(groupId, userId);
        }

        public async Task<IEnumerable<GroupDto>> GetGroupsByUserIdAsync(Guid userId)
        {
            var memberships = await groupMemberRepository.GetByUserIdAsync(userId);
            var groupIds = memberships.Select(m => m.GroupId);
            var groups = await groupRepository.GetAllAsync(userId);
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
