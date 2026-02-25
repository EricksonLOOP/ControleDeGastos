using System.Reflection.Emit;
using CGD.APP.DTOs.Group;
using CGD.Domain.Entities;

namespace CGD.APP.Mappers;

public static class GroupMemberMapper
{
    public static GroupMemberDto ToDto(GroupMember groupMembers)
    {
        return new GroupMemberDto
        {
            GroupId = groupMembers.GroupId,
            UserName = groupMembers.User.Name,
            UserId = groupMembers.UserId,
        };
    }
}