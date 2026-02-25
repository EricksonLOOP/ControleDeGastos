namespace CGD.APP.DTOs.User;

public class UserTotalsResponseDto
{
    public List<UserTotalsDto> UserTotals { get; set; } = new();
    public OverallTotalsDto OverallTotals { get; set; } = null!;
}
