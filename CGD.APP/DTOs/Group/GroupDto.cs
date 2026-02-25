using System.Collections.Generic;

namespace CGD.APP.DTOs.Group
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<GroupMemberDto> Members { get; set; }
    }
}
