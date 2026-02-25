using CGD.APP.DTOs.Group;
using CGD.Domain.Entities;

namespace CGD.APP.Mappers;

public static class GroupMapper
{
    public static GroupDto ToDto(Group group)
    {
        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Members = group.Members.Select(GroupMemberMapper.ToDto).ToList()
        };
    }
}