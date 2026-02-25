using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.APP.DTOs.Group;
using CGD.APP.Services.Groups;
using CGD.API.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CGD.API.Tests
{
    public class GroupsControllerTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var list = new List<GroupDto> { new() { Id = Guid.NewGuid() } };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.GetAllAsync(_userId)).ReturnsAsync(list);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            ok.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var dto = new GroupDto { Id = Guid.NewGuid() };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.GetByIdAsync(dto.Id, _userId)).ReturnsAsync(dto);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.GetById(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            ok.Value.Should().Be(dto);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), _userId)).ReturnsAsync((GroupDto)null);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.GetById(Guid.NewGuid());
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAt()
        {
            var dto = new GroupCreateDto { Name = "g" };
            var created = new GroupDto { Id = Guid.NewGuid(), Name = dto.Name };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.CreateAsync(dto, _userId)).ReturnsAsync(created);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.Create(dto);
            var createdAt = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            createdAt.ActionName.Should().Be(nameof(GroupsController.GetById));
            createdAt.Value.Should().Be(created);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenGroupExists()
        {
            var dto = new UpdateGroupDto { Name = "new" };
            var updated = new GroupDto { Id = Guid.NewGuid(), Name = dto.Name };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.UpdateAsync(updated.Id, dto.Name, _userId)).ReturnsAsync(updated);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.Update(updated.Id, dto);
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            ok.Value.Should().Be(updated);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenGroupMissing()
        {
            var dto = new UpdateGroupDto { Name = "x" };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), dto.Name, _userId)).ReturnsAsync((GroupDto)null);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.Update(Guid.NewGuid(), dto);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.DeleteAsync(_userId)).Returns(Task.CompletedTask);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var result = await controller.Delete(_userId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddUserToGroup_ReturnsNoContent()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.AddUserToGroupAsync(_userId, _userId, It.IsAny<Guid>())).Returns(Task.CompletedTask);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var result = await controller.AddUserToGroup(_userId, Guid.NewGuid());
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RemoveUserFromGroup_ReturnsNoContent()
        {
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.RemoveUserFromGroupAsync(_userId, It.IsAny<Guid>())).Returns(Task.CompletedTask);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var result = await controller.RemoveUserFromGroup(_userId, Guid.NewGuid());
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetGroupsByUser_ReturnsOk()
        {
            var list = new List<GroupDto> { new() { Id = Guid.NewGuid() } };
            var mock = new Mock<IGroupService>();
            mock.Setup(s => s.GetGroupsByUserIdAsync(_userId)).ReturnsAsync(list);
            var controller = ControllerTestHelpers.CreateWithUser<GroupsController>(_userId, mock.Object);

            var actionResult = await controller.GetGroupsByUser(_userId);
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            ok.Value.Should().BeEquivalentTo(list);
        }
    }
}