using CGD.APP.DTOs.Auth;
using CGD.APP.Services.Auth;
using CGD.APP.DTOs.User;
using ControleDeGastos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace ControleDeGastos.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthServices _authServices;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(ILogger<AuthController> logger, IAuthServices authServices, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtOptions)
    {
        _logger = logger;
        _authServices = authServices;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] AuthLoginDto authLoginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userDto = await _authServices.LoginAsync(authLoginDto);
        // TODO: Definir role real do usuário se houver, aqui "user" como padrão
        var token = _jwtTokenService.GenerateToken(userDto.Id, userDto.Email, "user", _jwtSettings.ExpirationMinutes);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? true,
            SameSite = SameSiteMode.Strict, // ou Lax se necessário
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Path = "/"
        };
        Response.Cookies.Append("AuthToken", token, cookieOptions);

        return Ok(new
        {
            userDto.Id,
            userDto.Name,
            userDto.BirthDate,
            userDto.Age,
            userDto.Email
        });
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignupUser([FromBody] AuthSignupDto authSignupDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _authServices.SignupAsync(authSignupDto);
        return Created("", null);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
        return NoContent();
    }
}