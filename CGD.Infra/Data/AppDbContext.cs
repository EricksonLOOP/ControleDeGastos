using CGD.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace CGD.Infra.Data;

public class CGDDbContext : DbContext
{
    public CGDDbContext(DbContextOptions<CGDDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<MonthlyBudget> MonthlyBudgets { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GroupMember>()
            .HasKey(gm => new { gm.GroupId, gm.UserId });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CGDDbContext).Assembly);
    }
}