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
public class AuthController(
    ILogger<AuthController> logger,
    IAuthServices authServices,
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtOptions)
    : ControllerBase
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginUser([FromBody] AuthLoginDto authLoginDto)
    {
        // Contrato: credenciais validas retornam 200 e gravam JWT no cookie HttpOnly.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userDto = await authServices.LoginAsync(authLoginDto);

        var token = jwtTokenService.GenerateToken(userDto.Id, userDto.Email, "USER", _jwtSettings.ExpirationMinutes);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) ?? true,
            SameSite = SameSiteMode.Strict,
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
        // Contrato: cria conta e retorna 201 sem payload de usuario autenticado.
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await authServices.SignupAsync(authSignupDto);
        return Created("", null);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Contrato: logout invalida o cookie de autenticacao e retorna 204.
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