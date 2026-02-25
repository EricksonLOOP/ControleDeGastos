using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGD.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CGD.Domain.Tests
{
    public class UserTests
    {
        private bool Validate(object model, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(model);
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Fact]
        public void Name_IsRequired()
        {
            var user = new User { Age = 30, BirthDate = DateTime.UtcNow, Email = "a@b.com", PasswordHash = "hash" };
            var valid = Validate(user, out var results);
            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(User.Name)));
        }

        [Fact]
        public void Email_IsRequired()
        {
            var user = new User { Name = "Test", Age = 30, BirthDate = DateTime.UtcNow, PasswordHash = "hash" };
            var valid = Validate(user, out var results);
            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(User.Email)));
        }

        [Fact]
        public void Name_HasMaxLength()
        {
            var user = new User
            {
                Name = new string('a', 201),
                Age = 30,
                BirthDate = DateTime.UtcNow,
                Email = "a@b.com",
                PasswordHash = "hash"
            };
            var valid = Validate(user, out var results);
            valid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(User.Name)));
        }

        [Fact]
        public void ValidUser_PassesValidation()
        {
            var user = new User
            {
                Name = "Alice",
                Age = 25,
                BirthDate = DateTime.Parse("1990-01-01"),
                Email = "alice@example.com",
                PasswordHash = "hash"
            };
            var valid = Validate(user, out var results);
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }
    }
}