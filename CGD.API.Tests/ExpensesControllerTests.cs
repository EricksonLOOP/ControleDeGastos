using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CGD.APP.DTOs.Expense;
using CGD.APP.Services.Expenses;
using ControleDeGastos.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CGD.API.Tests
{
    public class ExpensesControllerTests
    {
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public async Task Create_ReturnsCreatedAt_WhenModelValid()
        {
            var dto = new ExpenseCreateDto { Amount = 10, Description = "d" };
            var returned = new ExpenseDto { Id = Guid.NewGuid(), Amount = dto.Amount, Description = dto.Description };
            var mock = new Mock<IExpenseService>();
            mock.Setup(s => s.CreateAsync(dto, _userId)).ReturnsAsync(returned);
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, mock.Object);

            var result = await controller.Create(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            created.ActionName.Should().Be(nameof(ExpensesController.GetById));
            created.Value.Should().Be(returned);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, Mock.Of<IExpenseService>());
            ControllerTestHelpers.AddModelError(controller);

            var result = await controller.Create(new ExpenseCreateDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var dto = new ExpenseDto { Id = Guid.NewGuid() };
            var mock = new Mock<IExpenseService>();
            mock.Setup(s => s.GetByIdAsync(dto.Id)).ReturnsAsync(dto);
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, mock.Object);

            var result = await controller.GetById(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(dto);
        }

        [Fact]
        public async Task GetByUserId_ReturnsOk()
        {
            var list = new List<ExpenseDto> { new() { Id = Guid.NewGuid() } };
            var filter = new ExpenseFilterDto();
            var mock = new Mock<IExpenseService>();
            mock.Setup(s => s.GetByUserIdAsync(_userId, filter)).ReturnsAsync(list);
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, mock.Object);

            var result = await controller.GetByUserId(_userId, filter);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenModelValid()
        {
            var dto = new ExpenseUpdateDto { Amount = 20 };
            var updated = new ExpenseDto { Id = _userId, Amount = dto.Amount };
            var mock = new Mock<IExpenseService>();
            mock.Setup(s => s.UpdateAsync(_userId, _userId, dto)).ReturnsAsync(updated);
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, mock.Object);

            var result = await controller.Update(_userId, dto);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(updated);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, Mock.Of<IExpenseService>());
            ControllerTestHelpers.AddModelError(controller);

            var result = await controller.Update(_userId, new ExpenseUpdateDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var mock = new Mock<IExpenseService>();
            mock.Setup(s => s.DeleteAsync(_userId)).Returns(Task.CompletedTask);
            var controller = ControllerTestHelpers.CreateWithUser<ExpensesController>(_userId, mock.Object);

            var result = await controller.Delete(_userId);
            Assert.IsType<NoContentResult>(result);
        }
    }
}