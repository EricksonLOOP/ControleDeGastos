using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.User;

public class UserUpdateDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("age")]
    [Required(ErrorMessage = "Idade é obrigatória")]
    [Range(0, int.MaxValue, ErrorMessage = "Idade deve ser maior ou igual a zero")]
    public DateTime BirthDate { get; set; }
}
