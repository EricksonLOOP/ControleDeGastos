using CGD.APP.DTOs.Category;
using CGD.Domain.Entities;

namespace CGD.APP.Mappers;

public static class CategoryMapper
{
    public static ExpenseCategory ToEntity(Guid userId, CategoryCreateDto dto)
    {
        return new ExpenseCategory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Purpose = dto.Purpose
        };
    }

    public static CategoryDto ToDto(ExpenseCategory category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Purpose = category.Purpose
        };
    }
}
