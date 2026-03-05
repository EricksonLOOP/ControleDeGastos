using CGD.APP.DTOs.Expense;
using CGD.APP.Services.Expenses;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeGastos.Controllers;

[ApiController]
[Route("expenses")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    private readonly IExpenseService _expenseService = expenseService;


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExpenseCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var adminGroupId = GetUserId();
        var expense = await _expenseService.CreateAsync(dto, adminGroupId);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        return Ok(expense);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, [FromQuery] ExpenseFilterDto filter)
    {
        var authUserId = GetUserId();
        if (authUserId != userId)
            return Forbid();

        var expenses = await _expenseService.GetByUserIdAsync(userId, filter);
        return Ok(expenses);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ExpenseUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var adminId = GetUserId();
        var expense = await _expenseService.UpdateAsync(id, adminId, dto);
        return Ok(expense);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _expenseService.DeleteAsync(id);
        return NoContent();
    }
    private Guid GetUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException();
        return userId;
    }
}