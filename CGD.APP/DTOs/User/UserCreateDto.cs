using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.User;

public class UserCreateDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Name { get; set; } = null!;


    [JsonPropertyName("birthDate")]
    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime BirthDate { get; set; }

    [JsonPropertyName("email")]
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(8, ErrorMessage = "Password deve ter no mínimo 8 caracteres")]
    [MaxLength(25, ErrorMessage = "A senha é muito longa")]
    public string Password { get; set; } = null!;
}
