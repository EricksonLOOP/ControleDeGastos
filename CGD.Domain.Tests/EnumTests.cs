using CGD.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class EnumTests
    {
        [Fact]
        public void TransactionType_ValuesAreCorrect()
        {
            ((int)TransactionType.Expense).Should().Be(0);
            ((int)TransactionType.Income).Should().Be(1);
        }

        [Fact]
        public void CategoryPurpose_ValuesAreCorrect()
        {
            ((int)CategoryPurpose.Expense).Should().Be(0);
            ((int)CategoryPurpose.Income).Should().Be(1);
            ((int)CategoryPurpose.Both).Should().Be(2);
        }
    }
}