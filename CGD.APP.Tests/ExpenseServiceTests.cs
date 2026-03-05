using System;
using System.Threading.Tasks;
using CGD.APP.DTOs.Expense;
using CGD.APP.Services.Expenses;
using CGD.CrossCutting.Exceptions;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CGD.APP.Tests
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _expenseRepo = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IExpenseCategoryRepository> _categoryRepo = new();
        private readonly Mock<IGroupMemberRepository> _groupMemberRepo = new();
        private readonly ExpenseService _service;

        public ExpenseServiceTests()
        {
            _service = new ExpenseService(
                _expenseRepo.Object,
                _userRepo.Object,
                _categoryRepo.Object,
                _groupMemberRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ThrowsUserNotFound_WhenDebtorMissing()
        {
            // Arrange
            var dto = new ExpenseCreateDto { DebtorId = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Amount = 1, Description = "x", Type = TransactionType.Expense };
            _userRepo.Setup(r => r.GetByIdAsync(dto.DebtorId)).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _service.CreateAsync(dto, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidTransactionForMinor_WhenDebtorUnderageWithIncome()
        {
            var user = new User { Age = 17 };
            var dto = new ExpenseCreateDto { DebtorId = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Amount = 1, Description = "x", Type = TransactionType.Income };
            _userRepo.Setup(r => r.GetByIdAsync(dto.DebtorId)).ReturnsAsync(user);
            _categoryRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new ExpenseCategory { UserId = Guid.NewGuid(), Purpose = CategoryPurpose.Both });

            Func<Task> act = async () => await _service.CreateAsync(dto, Guid.NewGuid());
            await act.Should().ThrowAsync<InvalidTransactionForMinorException>();
        }

        [Fact]
        public async Task CreateAsync_ThrowsCategoryNotFound_WhenCategoryMissing()
        {
            var debtor = new User { Age = 20 };
            var dto = new ExpenseCreateDto { DebtorId = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Amount = 1, Description = "x", Type = TransactionType.Expense };
            _userRepo.Setup(r => r.GetByIdAsync(dto.DebtorId)).ReturnsAsync(debtor);
            _categoryRepo.Setup(r => r.GetByIdAsync(dto.CategoryId)).ReturnsAsync((ExpenseCategory?)null);

            Func<Task> act = async () => await _service.CreateAsync(dto, Guid.NewGuid());
            await act.Should().ThrowAsync<CategoryNotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidCategoryPurpose_WhenPurposeMismatches()
        {
            var debtor = new User { Age = 30 };
            var adminId = Guid.NewGuid();
            var dto = new ExpenseCreateDto { DebtorId = Guid.NewGuid(), CategoryId = Guid.NewGuid(), Amount = 1, Description = "x", Type = TransactionType.Expense };
            _userRepo.Setup(r => r.GetByIdAsync(dto.DebtorId)).ReturnsAsync(debtor);
            _categoryRepo.Setup(r => r.GetByIdAsync(dto.CategoryId)).ReturnsAsync(new ExpenseCategory { UserId = adminId, Purpose = CategoryPurpose.Income });

            Func<Task> act = async () => await _service.CreateAsync(dto, adminId);
            await act.Should().ThrowAsync<InvalidCategoryPurposeException>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsDto_WhenValid()
        {
            var adminId = Guid.NewGuid();
            var debtor = new User { Age = 25 };
            var dto = new ExpenseCreateDto { DebtorId = debtor.Id, CategoryId = Guid.NewGuid(), Amount = 1, Description = "x", Type = TransactionType.Expense };
            var category = new ExpenseCategory { UserId = adminId, Purpose = CategoryPurpose.Expense };

            _userRepo.Setup(r => r.GetByIdAsync(dto.DebtorId)).ReturnsAsync(debtor);
            _categoryRepo.Setup(r => r.GetByIdAsync(dto.CategoryId)).ReturnsAsync(category);
            _expenseRepo.Setup(r => r.AddAsync(It.IsAny<Expense>())).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto, adminId);

            result.Should().NotBeNull();
            result.Amount.Should().Be(dto.Amount);
            _expenseRepo.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Throws_WhenMissing()
        {
            _expenseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense?)null);
            Func<Task> act = async () => await _service.GetByIdAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<ExpenseNotFoundException>();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var expense = new Expense { Id = Guid.NewGuid(), Amount = 10 };
            _expenseRepo.Setup(r => r.GetByIdAsync(expense.Id)).ReturnsAsync(expense);

            var result = await _service.GetByIdAsync(expense.Id);
            result.Amount.Should().Be(expense.Amount);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsList()
        {
            _expenseRepo.Setup(r => r.GetByUserIdFilteredAsync(It.IsAny<Guid>(), It.IsAny<Domain.Filters.ExpenseFilter>()))
                        .ReturnsAsync(new List<Expense> { new Expense() });

            var list = await _service.GetByUserIdAsync(Guid.NewGuid());
            list.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenExpenseNotFound()
        {
            _expenseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense?)null);
            Func<Task> act = async () => await _service.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), new ExpenseUpdateDto());
            await act.Should().ThrowAsync<ExpenseNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenNotFound()
        {
            _expenseRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense?)null);
            Func<Task> act = async () => await _service.DeleteAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<ExpenseNotFoundException>();
        }

        [Fact]
        public async Task GetAll_ReturnsDtos()
        {
            var members = new List<CGD.Domain.Entities.GroupMember> { new CGD.Domain.Entities.GroupMember { User = new User { Id = Guid.NewGuid() } } };
            _groupMemberRepo.Setup(g => g.GetAllByGroupAdminId(It.IsAny<Guid>())).ReturnsAsync(members);
            _expenseRepo.Setup(r => r.GetByUserIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(new List<Expense> { new Expense { Amount = 5 } });

            var result = await _service.GetAll(Guid.NewGuid());
            result.Should().HaveCount(1);
        }
    }
}