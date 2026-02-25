using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGD.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class ExpenseTests
    {
        [Fact]
        public void Amount_CannotBeNegative()
        {
            // Arrange
            var expense = new Expense
            {
                Description = "Test",
                Amount = -5m,
                Type = TransactionType.Expense
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(expense);

            // Act
            var valid = Validator.TryValidateObject(expense, context, results, true);

            // Assert
            valid.Should().BeFalse("because negative values should not be allowed");
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Expense.Amount)));
        }

        [Fact]
        public void Description_IsRequired()
        {
            var expense = new Expense
            {
                Amount = 10m,
                Type = TransactionType.Expense
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(expense);
            var valid = Validator.TryValidateObject(expense, context, results, true);

            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Expense.Description)));
        }

        [Fact]
        public void Description_MaxLengthIsEnforced()
        {
            var expense = new Expense
            {
                Description = new string('d', 401),
                Amount = 10m,
                Type = TransactionType.Expense
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(expense);
            var valid = Validator.TryValidateObject(expense, context, results, true);

            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Expense.Description)));
        }

        // Enumerations are value types and cannot be null; the Required attribute
        // on `Type` will always pass because the default is `0` (Expense). This test
        // simply documents that behaviour and ensures the default maps to a valid value.
        [Fact]
        public void Type_DefaultsToExpense()
        {
            var expense = new Expense
            {
                Description = "Valid",
                Amount = 5m
                // Type left at default
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(expense);
            var valid = Validator.TryValidateObject(expense, context, results, true);

            valid.Should().BeTrue();
            expense.Type.Should().Be(TransactionType.Expense);
        }

        [Fact]
        public void ValidExpense_PassesValidation()
        {
            var expense = new Expense
            {
                Description = "Coffee",
                Amount = 2.5m,
                Type = TransactionType.Expense
            };
            var results = new List<ValidationResult>();
            var context = new ValidationContext(expense);
            var valid = Validator.TryValidateObject(expense, context, results, true);

            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}