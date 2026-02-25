using CGD.Domain.Entities;

namespace CGD.APP.DTOs.Category;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public CategoryPurpose Purpose { get; set; }
}
