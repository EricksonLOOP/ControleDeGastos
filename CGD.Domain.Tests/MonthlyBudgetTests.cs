using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGD.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class MonthlyBudgetTests
    {
        private bool Validate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Limit_CanBeZeroOrPositive()
        {
            var budget = new MonthlyBudget { Year = 2025, Month = 5, Limit = 0m };
            var valid = Validate(budget, out var results);
            valid.Should().BeTrue();
            results.Should().BeEmpty();

            budget.Limit = 100m;
            valid = Validate(budget, out results);
            valid.Should().BeTrue();
        }

        [Fact]
        public void DefaultValues_AreAcceptable()
        {
            var budget = new MonthlyBudget();
            var valid = Validate(budget, out var results);
            // there are no validation attributes, the object is always considered valid
            valid.Should().BeTrue();
        }
    }
}