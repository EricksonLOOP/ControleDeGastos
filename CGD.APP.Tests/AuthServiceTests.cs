using System;
using System.Threading.Tasks;
using CGD.APP.DTOs.Auth;
using CGD.APP.Services.Auth;
using CGD.APP.DTOs.User;
using CGD.APP.Services.Auth;
using CGD.CrossCutting.Exceptions;
using CGD.CrossCutting.Security;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CGD.APP.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly PasswordHash _passwordHash = new();
        private readonly AuthServices _service;

        public AuthServiceTests()
        {
            _service = new AuthServices(_userRepo.Object, _passwordHash);
        }

        [Fact]
        public async Task SignupAsync_ThrowsArgument_WhenEmailAlreadyExists()
        {
            var dto = new AuthSignupDto { Email = "a@b.com", Password = "pwd" };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(new User());

            Func<Task> act = async () => await _service.SignupAsync(dto);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Email já cadastrado");
        }

        [Fact]
        public async Task SignupAsync_CallsAdd_WhenNewEmail()
        {
            var dto = new AuthSignupDto { Email = "a@b.com", Password = "pwd" };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            await _service.SignupAsync(dto);

            _userRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == dto.Email &&
                                                                 _passwordHash.Verify(dto.Password, u.PasswordHash))),
                                 Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ThrowsUserNotFound_WhenEmailMissing()
        {
            var dto = new AuthLoginDto { email = "x@y.com", Password = "123" };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync((User?)null);

            Func<Task> act = async () => await _service.LoginAsync(dto);
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task LoginAsync_ThrowsUnauthorized_WhenPasswordIncorrect()
        {
            var dto = new AuthLoginDto { email = "x@y.com", Password = "bad" };
            var user = new User { PasswordHash = _passwordHash.HashPassword("good") };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);

            Func<Task> act = async () => await _service.LoginAsync(dto);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task LoginAsync_ReturnsDto_OnSuccess()
        {
            var dto = new AuthLoginDto { email = "x@y.com", Password = "pwd" };
            var user = new User { Id = Guid.NewGuid(), Name = "Name", Email = dto.email, PasswordHash = _passwordHash.HashPassword(dto.Password) };
            _userRepo.Setup(r => r.GetByEmailAsync(dto.email)).ReturnsAsync(user);

            var result = await _service.LoginAsync(dto);

            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
        }
    }
}