using System;
using System.Threading.Tasks;
using CGD.APP.DTOs.Auth;
using CGD.APP.DTOs.User;
using CGD.APP.Services.Auth;
using ControleDeGastos;
using ControleDeGastos.Controllers;
using ControleDeGastos.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CGD.API.Tests
{
    public class AuthControllerTests
    {
        private readonly JwtSettings _jwtSettings = new() { ExpirationMinutes = 60 };

        [Fact]
        public async Task LoginUser_ReturnsOkAndSetsCookie_WhenCredentialsValid()
        {
            // arrange
            var loginDto = new AuthLoginDto { email = "test@example.com", Password = "pass" };
            var userDto = new UserDto { Id = Guid.NewGuid(), Email = loginDto.email, Name = "Name", BirthDate = DateTime.UtcNow };

            var mockAuth = new Mock<IAuthServices>();
            mockAuth.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(userDto);

            var mockJwt = new Mock<IJwtTokenService>();
            mockJwt.Setup(s => s.GenerateToken(userDto.Id, userDto.Email, "user", It.IsAny<int>()))
                   .Returns("token");

            var controller = new AuthController(Mock.Of<ILogger<AuthController>>(), mockAuth.Object, mockJwt.Object, Options.Create(_jwtSettings));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // act
            var result = await controller.LoginUser(loginDto);

            // assert
            var ok = Assert.IsType<OkObjectResult>(result);
            ok.Value.Should().BeEquivalentTo(new
            {
                userDto.Id,
                userDto.Name,
                userDto.BirthDate,
                userDto.Age,
                userDto.Email
            });

            mockJwt.Verify(s => s.GenerateToken(userDto.Id, userDto.Email, "user", It.IsAny<int>()), Times.Once);
            // Cookie no headr para testar
            Assert.Contains("AuthToken", controller.Response.Headers["Set-Cookie"].ToString());
        }

        [Fact]
        public async Task LoginUser_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = new AuthController(Mock.Of<ILogger<AuthController>>(), Mock.Of<IAuthServices>(), Mock.Of<IJwtTokenService>(), Options.Create(_jwtSettings))
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };
            controller.ModelState.AddModelError("Email", "Required");

            var result = await controller.LoginUser(new AuthLoginDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SignupUser_ReturnsCreated_WhenModelValid()
        {
            var signupDto = new AuthSignupDto { Email = "a@b.com", Name = "Name", Password = "pwd" };
            var mockAuth = new Mock<IAuthServices>();
            mockAuth.Setup(s => s.SignupAsync(signupDto)).Returns(Task.CompletedTask);

            var controller = new AuthController(Mock.Of<ILogger<AuthController>>(), mockAuth.Object, Mock.Of<IJwtTokenService>(), Options.Create(_jwtSettings))
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };

            var result = await controller.SignupUser(signupDto);
            Assert.IsType<CreatedResult>(result);
            mockAuth.Verify(s => s.SignupAsync(signupDto), Times.Once);
        }

        [Fact]
        public async Task SignupUser_ReturnsBadRequest_WhenModelInvalid()
        {
            var controller = new AuthController(Mock.Of<ILogger<AuthController>>(), Mock.Of<IAuthServices>(), Mock.Of<IJwtTokenService>(), Options.Create(_jwtSettings))
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };
            controller.ModelState.AddModelError("Email", "Required");

            var result = await controller.SignupUser(new AuthSignupDto());
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Logout_ReturnsNoContent_AndDeletesCookie()
        {
            var controller = new AuthController(Mock.Of<ILogger<AuthController>>(), Mock.Of<IAuthServices>(), Mock.Of<IJwtTokenService>(), Options.Create(_jwtSettings))
                {
                    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
                };
            controller.Response.Cookies.Append("AuthToken", "dummy");

            var result = controller.Logout();
            Assert.IsType<NoContentResult>(result);

            var header = controller.Response.Headers["Set-Cookie"].ToString();
            Assert.Contains("expires=Thu, 01 Jan 1970", header);
        }
    }
}