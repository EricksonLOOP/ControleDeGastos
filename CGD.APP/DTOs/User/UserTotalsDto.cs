namespace CGD.APP.DTOs.User;

public class UserTotalsDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
}
