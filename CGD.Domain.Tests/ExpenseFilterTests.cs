using CGD.Domain.Filters;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class ExpenseFilterTests
    {
        [Fact]
        public void Properties_CanBeAssignedAndRead()
        {
            var filter = new ExpenseFilter
            {
                CategoryId = System.Guid.NewGuid(),
                StartDate = System.DateTime.Today.AddDays(-7),
                EndDate = System.DateTime.Today,
                MinAmount = 1m,
                MaxAmount = 100m,
                DescriptionContains = "rent"
            };

            filter.CategoryId.Should().HaveValue();
            filter.StartDate.Should().HaveValue();
            filter.EndDate.Should().HaveValue();
            filter.MinAmount.Should().Be(1m);
            filter.MaxAmount.Should().Be(100m);
            filter.DescriptionContains.Should().Be("rent");
        }
    }
}