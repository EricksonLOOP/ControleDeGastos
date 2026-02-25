using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGD.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class ExpenseCategoryTests
    {
        private bool Validate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Name_MaxLengthExceeded_Fails()
        {
            var category = new ExpenseCategory
            {
                Name = new string('x', 101),
                Description = "Desc",
                Purpose = CategoryPurpose.Expense
            };
            var valid = Validate(category, out var results);
            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(ExpenseCategory.Name)));
        }

        [Fact]
        public void Description_MaxLengthExceeded_Fails()
        {
            var category = new ExpenseCategory
            {
                Name = "Name",
                Description = new string('y', 401),
                Purpose = CategoryPurpose.Expense
            };
            var valid = Validate(category, out var results);
            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(ExpenseCategory.Description)));
        }

        [Fact]
        public void ValidCategory_PassesValidation()
        {
            var category = new ExpenseCategory
            {
                Name = "Groceries",
                Description = "Food and supplies",
                Purpose = CategoryPurpose.Both
            };
            var valid = Validate(category, out var results);
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}