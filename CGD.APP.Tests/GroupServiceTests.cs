using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;
using CGD.APP.Services.Groups;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CGD.APP.Tests
{
    public class GroupServiceTests
    {
        private readonly Mock<IGroupRepository> _groupRepo = new();
        private readonly Mock<IGroupMemberRepository> _memberRepo = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly GroupService _service;

        public GroupServiceTests()
        {
            _service = new GroupService(_groupRepo.Object, _memberRepo.Object, _userRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _groupRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                      .ReturnsAsync((Group?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenFound()
        {
            var group = new Group { Id = Guid.NewGuid(), Name = "x", Members = new List<GroupMember>() };
            _groupRepo.Setup(r => r.GetByIdAsync(group.Id, It.IsAny<Guid>()))
                      .ReturnsAsync(group);

            var result = await _service.GetByIdAsync(group.Id, Guid.NewGuid());
            result.Name.Should().Be(group.Name);
        }

        [Fact]
        public async Task CreateAsync_AddsGroupAndReturnsDto()
        {
            var dto = new GroupCreateDto { Name = "new" };
            Group added = null!;
            _groupRepo.Setup(r => r.AddAsync(It.IsAny<Group>())).Callback<Group>(g => added = g).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto, Guid.NewGuid());
            result.Name.Should().Be(dto.Name);
            added.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenMissing()
        {
            _groupRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                      .ReturnsAsync((Group?)null);
            var result = await _service.UpdateAsync(Guid.NewGuid(), "name", Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ChangesName_WhenExists()
        {
            var group = new Group { Id = Guid.NewGuid(), Name = "old" };
            _groupRepo.Setup(r => r.GetByIdAsync(group.Id, It.IsAny<Guid>()))
                      .ReturnsAsync(group);
            _groupRepo.Setup(r => r.UpdateAsync(group)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(group.Id, "new", Guid.NewGuid());
            result.Name.Should().Be("new");
            group.Name.Should().Be("new");
        }

        [Fact]
        public async Task DeleteAsync_CallsRepository()
        {
            await _service.DeleteAsync(Guid.NewGuid());
            _groupRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task AddUserToGroupAsync_Throws_WhenGroupMissingOrUserMissing()
        {
            _groupRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                      .ReturnsAsync((Group?)null);
            _userRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            Func<Task> act = async () => await _service.AddUserToGroupAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task AddUserToGroupAsync_CallsRepo_OnSuccess()
        {
            var group = new Group { Id = Guid.NewGuid() };
            var user = new User { Id = Guid.NewGuid() };
            _groupRepo.Setup(r => r.GetByIdAsync(group.Id, It.IsAny<Guid>())).ReturnsAsync(group);
            _userRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _memberRepo.Setup(r => r.AddAsync(It.IsAny<GroupMember>())).Returns(Task.CompletedTask);

            await _service.AddUserToGroupAsync(group.Id, Guid.NewGuid(), user.Id);
            _memberRepo.Verify(r => r.AddAsync(It.Is<GroupMember>(m => m.GroupId == group.Id && m.UserId == user.Id)), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromGroupAsync_CallsRepo()
        {
            await _service.RemoveUserFromGroupAsync(Guid.NewGuid(), Guid.NewGuid());
            _memberRepo.Verify(r => r.RemoveAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetGroupsByUserIdAsync_ReturnsFilteredGroups()
        {
            var membership = new GroupMember { GroupId = Guid.NewGuid() };
            var groups = new List<Group> { new Group { Id = membership.GroupId } };
            _memberRepo.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(new[] { membership });
            _groupRepo.Setup(r => r.GetAllAsync(It.IsAny<Guid>())).ReturnsAsync(groups);

            var result = await _service.GetGroupsByUserIdAsync(Guid.NewGuid());
            result.Should().HaveCount(1);
        }
    }
}