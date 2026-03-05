using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CGD.APP.DTOs.User;
using CGD.APP.Services.Users;
using CGD.API.Controllers;
using ControleDeGastos.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CGD.API.Tests
{
    public class UsersControllerTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public async Task CreateSimple_ReturnsOk_WhenModelValid()
        {
            var dto = new UserSimpleCreateDto { Name = "n", BirthDate = DateTime.UtcNow, GroupId = Guid.NewGuid() };
            var resultDto = new UserDto { Id = Guid.NewGuid(), Name = dto.Name, BirthDate = dto.BirthDate, Email = "user@example.com" };
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.CreateSimpleAsync(dto, _userId)).ReturnsAsync(resultDto);

            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var action = await controller.CreateSimple(dto);
            var ok = Assert.IsType<OkObjectResult>(action);
            ok.Value.Should().BeEquivalentTo(new
            {
                resultDto.Id,
                resultDto.Name,
                resultDto.BirthDate,
                resultDto.Email
            });
        }

        [Fact]
        public async Task CreateSimple_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, Mock.Of<IUserService>());
            ControllerTestHelpers.AddModelError(controller);

            var result = await controller.CreateSimple(new UserSimpleCreateDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetMe_ReturnsNotFound_WhenUserMissing()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetByIdAsync(_userId)).ReturnsAsync((UserDto)null);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.GetMe();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var dto = new UserDto { Id = Guid.NewGuid(), Name = "x" };
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetByIdAsync(dto.Id)).ReturnsAsync(dto);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.GetById(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(dto);
        }

        [Fact]
        public async Task GetPaged_ReturnsOk_WithPageData()
        {
            var pageResult = new List<UserDto> { new() { Id = Guid.NewGuid() } };
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetPagedByCommonGroupsAsync(_userId, 1, 20)).ReturnsAsync(pageResult);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.GetPaged();
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().BeEquivalentTo(pageResult);
        }

        [Fact]
        public async Task GetUserTotals_ReturnsOk()
        {
            var totals = new UserTotalsResponseDto();
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.GetUserTotalsAsync(_userId)).ReturnsAsync(totals);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.GetUserTotals();
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(totals);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenModelValid()
        {
            var dto = new UserUpdateDto { Name = "n" };
            var updated = new UserDto { Id = _userId, Name = dto.Name };
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.UpdateAsync(_userId, dto)).ReturnsAsync(updated);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.Update(_userId, dto);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(updated);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, Mock.Of<IUserService>());
            ControllerTestHelpers.AddModelError(controller);

            var result = await controller.Update(_userId, new UserUpdateDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var mock = new Mock<IUserService>();
            mock.Setup(s => s.DeleteAsync(_userId)).Returns(Task.CompletedTask);
            var controller = ControllerTestHelpers.CreateWithUser<UsersController>(_userId, mock.Object);

            var result = await controller.Delete(_userId);
            Assert.IsType<NoContentResult>(result);
        }

    }
}