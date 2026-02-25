using System.ComponentModel.DataAnnotations.Schema;

namespace CGD.Domain.Entities
{
    public class GroupMember
    {
        [System.ComponentModel.DataAnnotations.Key, Column(Order = 0)]
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        [System.ComponentModel.DataAnnotations.Key, Column(Order = 1)]
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
