using CGD.APP.DTOs.User;

namespace CGD.APP.Services.Users;

public interface IUserService

{
    Task<UserDto> CreateSimpleAsync(UserSimpleCreateDto dto, Guid adminGroupId);
    Task<UserDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<UserDto>> GetPagedAsync(int page, int pageSize);
    Task<IReadOnlyList<UserDto>> GetPagedByCommonGroupsAsync(Guid userId, int page, int pageSize);
    Task<UserDto> CreateAsync(UserCreateDto dto);
    Task<UserDto> UpdateAsync(Guid id, UserUpdateDto dto);
    Task DeleteAsync(Guid id);
    Task<UserTotalsResponseDto> GetUserTotalsAsync();
    Task<List<EnrichedUserDto>> GetAllEnrichedUsers(Guid adminGroupId);
}

