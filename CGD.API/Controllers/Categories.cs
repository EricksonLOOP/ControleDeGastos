using CGD.APP.DTOs.Category;
using CGD.APP.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeGastos.Controllers;

[ApiController]
[Route("categories")]
[Microsoft.AspNetCore.Authorization.Authorize]
public class CategoriesController(IExpenseCategoryService categoryService) : ControllerBase
{
    private readonly IExpenseCategoryService _categoryService = categoryService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var category = await _categoryService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var categories = await _categoryService.GetByUserIdAsync(userId);
        return Ok(categories);
    }
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var authUserId = GetUserId();
        if (authUserId != userId)
            return Forbid();

        var categories = await _categoryService.GetByUserIdAsync(userId);
        return Ok(categories);
    }

    [HttpGet("user/{userId:guid}/paged")]
    public async Task<IActionResult> GetPagedByUserId(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var authUserId = GetUserId();
        if (authUserId != userId)
            return Forbid();

        var categories = await _categoryService.GetPagedByUserIdAsync(userId, page, pageSize);
        return Ok(categories);
    }

    [HttpGet("user/{userId:guid}/totals")]
    public async Task<IActionResult> GetCategoryTotals(Guid userId)
    {
        var authUserId = GetUserId();
        if (authUserId != userId)
            return Forbid();

        var totals = await _categoryService.GetCategoryTotalsAsync(userId);
        return Ok(totals);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = await _categoryService.UpdateAsync(id, dto);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteAsync(id);
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
