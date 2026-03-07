using CGD.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.Category;

public class CategoryCreateDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    // Restricao declarativa de tamanho para manter nome curto e indexavel.
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("description")]
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(400, MinimumLength = 5, ErrorMessage = "Descrição deve ter entre 5 e 400 caracteres")]
    public string Description { get; set; } = null!;
    [JsonPropertyName("purpose")]
    [Required(ErrorMessage = "Finalidade é obrigatória")]
    // Purpose e obrigatoria para validar compatibilidade com TransactionType.
    public CategoryPurpose Purpose { get; set; }
}
