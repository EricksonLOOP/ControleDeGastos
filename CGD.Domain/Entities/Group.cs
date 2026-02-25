using System.Collections.Generic;

namespace CGD.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public Guid CreatedBy { get; set; }
        public ICollection<GroupMember> Members { get; set; }
    }
}
