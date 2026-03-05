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

    [JsonPropertyName("birthDate")]
    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime BirthDate { get; set; }
}
