using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;
using CGD.APP.DTOs.User;
using CGD.APP.DTOs.Group;
using CGD.APP.Services.Groups;
using CGD.APP.Services.Users;
using CGD.CrossCutting.Exceptions;
using CGD.CrossCutting.Security;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CGD.APP.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IExpenseRepository> _expenseRepo = new();
        private readonly Mock<IGroupMemberRepository> _gmRepo = new();
        private readonly PasswordHash _passwordHash = new();
        private readonly Mock<IGroupService> _groupService = new();
        private readonly UserService _service;

        public UserServiceTests()
        {
            _service = new UserService(_userRepo.Object, _expenseRepo.Object, _gmRepo.Object, _passwordHash, _groupService.Object);
        }

        [Fact]
        public async Task CreateSimpleAsync_AddsUserAndGroupMember()
        {
            var dto = new UserSimpleCreateDto { Name = "u", BirthDate = DateTime.Today.AddYears(-20), GroupId = Guid.NewGuid() };
            var createdUser = new User { Id = Guid.NewGuid(), BirthDate = dto.BirthDate };
            _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
            _groupService.Setup(s => s.GetByIdAsync(dto.GroupId, It.IsAny<Guid>())).ReturnsAsync(new GroupDto { Id = dto.GroupId });
            _groupService.Setup(s => s.AddUserToGroupAsync(dto.GroupId, It.IsAny<Guid>(), createdUser.Id)).Returns(Task.CompletedTask);

            var result = await _service.CreateSimpleAsync(dto, Guid.NewGuid());
            result.Name.Should().Be(dto.Name);
            _groupService.Verify(s => s.AddUserToGroupAsync(dto.GroupId, It.IsAny<Guid>(), createdUser.Id), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Throws_WhenMissing()
        {
            _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
            Func<Task> act = async () => await _service.GetByIdAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetPagedAsync_Throws_WhenBadParams()
        {
            Func<Task> act1 = async () => await _service.GetPagedAsync(0, 10);
            await act1.Should().ThrowAsync<ArgumentException>();
            Func<Task> act2 = async () => await _service.GetPagedAsync(1, 0);
            await act2.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenEmailExists()
        {
            var dto = new UserCreateDto { Email = "x@x.com", Password = "p" };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(new User());
            Func<Task> act = async () => await _service.CreateAsync(dto);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task CreateAsync_ReturnsDto_OnSuccess()
        {
            var dto = new UserCreateDto { Name = "u", Email = "e@e.com", Password = "p", BirthDate = DateTime.Today.AddYears(-30) };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = Guid.NewGuid(), BirthDate = dto.BirthDate });

            var result = await _service.CreateAsync(dto);
            result.Email.Should().Be(dto.Email);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenMissing()
        {
            _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
            Func<Task> act = async () => await _service.UpdateAsync(Guid.NewGuid(), new UserUpdateDto());
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAge_WhenBirthdateChanged()
        {
            var user = new User { Id = Guid.NewGuid(), BirthDate = DateTime.Today.AddYears(-20), Name = "old" };
            _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _userRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
            var dto = new UserUpdateDto { Name = "new", BirthDate = DateTime.Today.AddYears(-25) };

            var result = await _service.UpdateAsync(user.Id, dto);
            result.Name.Should().Be("new");
            user.Age.Should().BeLessThan(30);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenMissing()
        {
            _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
            Func<Task> act = async () => await _service.DeleteAsync(Guid.NewGuid());
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task GetUserTotalsAsync_CalculatesTotals()
        {
            // expenses must include User reference for grouping
            var userA = new User { Id = Guid.NewGuid(), Name = "Alice" };
            var userB = new User { Id = Guid.NewGuid(), Name = "Bob" };
            var expenses = new List<Expense>
            {
                new Expense { UserId = userA.Id, User = userA, Type = TransactionType.Income, Amount = 100 },
                new Expense { UserId = userB.Id, User = userB, Type = TransactionType.Expense, Amount = 40 }
            };
            _expenseRepo.Setup(r => r.GetAllWithUsersAsync()).ReturnsAsync(expenses);

            var result = await _service.GetUserTotalsAsync();
            result.OverallTotals.TotalIncome.Should().Be(100);
        }

        [Fact]
        public async Task GetPagedByCommonGroupsAsync_ReturnsEmpty_WhenNoMembers()
        {
            _groupService.Setup(g => g.GetAllAsync(It.IsAny<Guid>())).ReturnsAsync(new List<GroupDto>());
            var result = await _service.GetPagedByCommonGroupsAsync(Guid.NewGuid(), 1, 10);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllEnrichedUsers_ReturnsUsersWithExpenses()
        {
            var adminId = Guid.NewGuid();
            var enriched = new List<User> { new User { Id = Guid.NewGuid(), Name = "u", GroupMembers = new List<CGD.Domain.Entities.GroupMember> { new CGD.Domain.Entities.GroupMember { Group = new Group { Id = Guid.NewGuid(), Name = "g", Members = new List<GroupMember>() } } } } };
            _userRepo.Setup(r => r.GetEnrichedUsers(adminId)).ReturnsAsync(enriched);
            var expenses = new List<Expense> { new Expense { DebtorId = enriched[0].Id, Amount = 10 } };
            _expenseRepo.Setup(r => r.GetByDebtorIdsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(expenses);

            var result = await _service.GetAllEnrichedUsers(adminId);
            result.Should().HaveCount(1);
            result[0].Expenses.Should().HaveCount(1);
        }
    }
}