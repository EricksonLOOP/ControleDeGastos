using CGD.APP.DTOs.User;
using CGD.APP.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeGastos.Controllers;

[ApiController]
[Route("users")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;



    [HttpPost("simple")]
    public async Task<IActionResult> CreateSimple([FromBody] UserSimpleCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        // Escopo de seguranca: o usuario autenticado define o admin responsavel pela nova pessoa.
        var userId = GetUserId();
        var userCreated = await _userService.CreateSimpleAsync(dto, userId);
        return Ok(new
        {
            userCreated.Id,
            userCreated.Name,
            userCreated.BirthDate,
            userCreated.Age,
            userCreated.Email
        });
    }

    [HttpGet("all/enriched")]
    public async Task<IActionResult> GetEnrichedUsers()
    {
        var adminGroupId = GetUserId();
        var enrichedUsers = await _userService.GetAllEnrichedUsers(adminGroupId);
        return Ok(enrichedUsers);

    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();

        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
        // Unauthorized indica ausencia/invalidade de identidade; Forbid seria usado quando identidade existe sem permissao.
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var users = await _userService.GetPagedByCommonGroupsAsync(userId, page, pageSize);
        return Ok(users);
    }

    [HttpGet("totals")]
    public async Task<IActionResult> GetUserTotals()
    {
        // Escopo dos totais: membros acessiveis ao admin autenticado + consolidado geral.
        var userId = GetUserId();
        var totals = await _userService.GetUserTotalsAsync(userId);
        return Ok(totals);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.UpdateAsync(id, dto);
        return Ok(user);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }

    private Guid GetUserId()
    {
        // Aceita NameIdentifier ou sub para compatibilidade entre emissores JWT.
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException();
        return userId;
    }
}
