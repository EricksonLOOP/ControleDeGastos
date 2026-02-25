using CGD.APP.DTOs.Auth;
using CGD.APP.DTOs.User;
using CGD.Domain.Entities;

namespace CGD.APP.Mappers;

public abstract class UserMapper
{
    public static User SimpleCreateToEntity(UserSimpleCreateDto dto)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc),
            Email = string.Empty,
            PasswordHash = string.Empty,
        };
    }
    public static User AuthToEntity(AuthSignupDto dto, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            Email = dto.Email,
            PasswordHash = passwordHash
        };
    }

    public static User CreateToEntity(UserCreateDto dto, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            Email = dto.Email,
            PasswordHash = passwordHash
        };
    }

    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            BirthDate = user.BirthDate,
            Email = user.Email,
        };
    }
}