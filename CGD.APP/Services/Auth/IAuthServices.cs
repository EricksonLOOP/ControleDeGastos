using CGD.APP.DTOs.Auth;
using CGD.APP.DTOs.User;

namespace CGD.APP.Services.Auth;

public interface IAuthServices
{
    Task SignupAsync(AuthSignupDto authSignupDto);
    Task<UserDto> LoginAsync(AuthLoginDto authLoginDto);
}
