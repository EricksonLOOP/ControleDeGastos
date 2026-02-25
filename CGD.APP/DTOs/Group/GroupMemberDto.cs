namespace CGD.APP.DTOs.Group
{
    public class GroupMemberDto
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
