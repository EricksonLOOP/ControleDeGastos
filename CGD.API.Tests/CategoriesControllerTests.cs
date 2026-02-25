using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CGD.APP.DTOs.Category;
using CGD.APP.Services.Categories;
using CGD.Domain.Entities;
using ControleDeGastos.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CGD.API.Tests
{
    public class CategoriesControllerTests
    {
        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenModelValid()
        {
            // arrange
            var userId = Guid.NewGuid();
            var dto = new CategoryCreateDto { Name = "Test", Purpose = CategoryPurpose.Expense };
            var returned = new CategoryDto { Id = Guid.NewGuid(), Name = dto.Name, Purpose = dto.Purpose };

            var mock = new Mock<IExpenseCategoryService>();
            mock
                .Setup(s => s.CreateAsync(userId, dto))
                .ReturnsAsync(returned);

            var controller = new CategoriesController(mock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    ], "TestAuthentication"))
                }
            };

            // act
            var result = await controller.Create(dto);

            // assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            created.ActionName.Should().Be(nameof(CategoriesController.GetById));
            created.Value.Should().Be(returned);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelInvalid()
        {
            var mock = new Mock<IExpenseCategoryService>();
            var controller = new CategoriesController(mock.Object);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.Create(new CategoryCreateDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var dto = new CategoryDto { Id = id, Name = "x", Purpose = CategoryPurpose.Expense };
            var mock = new Mock<IExpenseCategoryService>();
            mock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(dto);
            var controller = new CategoriesController(mock.Object);

            var result = await controller.GetById(id);
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().Be(dto);
        }

        [Fact]
        public async Task GetById_ReturnsOkWithNull_WhenMissing()
        {
            var mock = new Mock<IExpenseCategoryService>();
            mock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))!.ReturnsAsync((CategoryDto)null!);
            var controller = new CategoriesController(mock.Object);

            var result = await controller.GetById(Guid.NewGuid());
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().BeNull();
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenClaimMissing()
        {
            var dto = new CategoryCreateDto { Name = "x", Purpose = CategoryPurpose.Expense };
            var controller = new CategoriesController(Mock.Of<IExpenseCategoryService>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.Create(dto);
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}