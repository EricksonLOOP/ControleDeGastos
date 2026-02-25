using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGD.APP.DTOs.Category;
using CGD.APP.Services.Categories;
using CGD.CrossCutting.Exceptions;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CGD.APP.Tests
{
    public class ExpenseCategoryServiceTests
    {
        private readonly Mock<IExpenseCategoryRepository> _catRepo = new();
        private readonly Mock<IExpenseRepository> _expenseRepo = new();
        private readonly ExpenseCategoryService _service;

        public ExpenseCategoryServiceTests()
        {
            _service = new ExpenseCategoryService(_catRepo.Object, _expenseRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsDto()
        {
            var userId = Guid.NewGuid();
            var dto = new CategoryCreateDto { Name = "c", Description = "d", Purpose = CategoryPurpose.Expense };
            _catRepo.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>())).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(userId, dto);

            result.Name.Should().Be(dto.Name);
            _catRepo.Verify(r => r.AddAsync(It.Is<ExpenseCategory>(c => c.UserId == userId && c.Name == dto.Name)), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsArgument_WhenNull()
        {
            _catRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseCategory?)null);

            Func<Task> act = async () => await _service.GetByIdAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var cat = new ExpenseCategory { Id = Guid.NewGuid(), Name = "n", Description = "d", Purpose = CategoryPurpose.Both };
            _catRepo.Setup(r => r.GetByIdAsync(cat.Id)).ReturnsAsync(cat);

            var result = await _service.GetByIdAsync(cat.Id);
            result.Name.Should().Be(cat.Name);
        }

        [Fact]
        public async Task GetPagedByUserIdAsync_Throws_WhenInvalidPage()
        {
            Func<Task> act1 = async () => await _service.GetPagedByUserIdAsync(Guid.NewGuid(), 0, 10);
            await act1.Should().ThrowAsync<ArgumentException>();

            Func<Task> act2 = async () => await _service.GetPagedByUserIdAsync(Guid.NewGuid(), 1, 0);
            await act2.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetPagedByUserIdAsync_ReturnsList()
        {
            var list = new List<ExpenseCategory> { new ExpenseCategory { Id = Guid.NewGuid() } };
            _catRepo.Setup(r => r.GetPagedByUserIdAsync(It.IsAny<Guid>(), 1, 10)).ReturnsAsync(list);

            var result = await _service.GetPagedByUserIdAsync(Guid.NewGuid(), 1, 10);
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenNotFound()
        {
            _catRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseCategory?)null);
            Func<Task> act = async () => await _service.UpdateAsync(Guid.NewGuid(), new CategoryCreateDto());
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateAsync_ChangesValues_WhenFound()
        {
            var cat = new ExpenseCategory { Id = Guid.NewGuid(), Name = "old", Description = "old", Purpose = CategoryPurpose.Both };
            _catRepo.Setup(r => r.GetByIdAsync(cat.Id)).ReturnsAsync(cat);
            _catRepo.Setup(r => r.UpdateAsync(cat)).Returns(Task.CompletedTask);
            var dto = new CategoryCreateDto { Name = "new", Description = "new", Purpose = CategoryPurpose.Expense };

            var result = await _service.UpdateAsync(cat.Id, dto);
            result.Name.Should().Be(dto.Name);
            cat.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenNotFound()
        {
            _catRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ExpenseCategory?)null);
            Func<Task> act = async () => await _service.DeleteAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task DeleteAsync_CallsRepo_WhenFound()
        {
            var cat = new ExpenseCategory { Id = Guid.NewGuid() };
            _catRepo.Setup(r => r.GetByIdAsync(cat.Id)).ReturnsAsync(cat);
            _catRepo.Setup(r => r.DeleteAsync(cat)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(cat.Id);
            _catRepo.Verify(r => r.DeleteAsync(cat), Times.Once);
        }

        [Fact]
        public async Task GetCategoryTotalsAsync_CalculatesCorrectly()
        {
            var userId = Guid.NewGuid();
            var cat1 = new ExpenseCategory { Id = Guid.NewGuid(), Name = "c1" };
            var cat2 = new ExpenseCategory { Id = Guid.NewGuid(), Name = "c2" };
            var exp = new List<Expense>
            {
                new Expense { CategoryId = cat1.Id, Type = TransactionType.Income, Amount = 100 },
                new Expense { CategoryId = cat1.Id, Type = TransactionType.Expense, Amount = 40 },
                new Expense { CategoryId = cat2.Id, Type = TransactionType.Expense, Amount = 20 },
            };

            _catRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new[] { cat1, cat2 });
            _expenseRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(exp);

            var result = await _service.GetCategoryTotalsAsync(userId);

            result.Categories.Should().HaveCount(2);
            result.GrandTotalIncome.Should().Be(100);
            result.GrandTotalExpenses.Should().Be(60);
        }
    }
}