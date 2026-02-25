using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.Auth;

public class AuthLoginDto
{
    [JsonPropertyName("email")]
    [Required(ErrorMessage = "É necessário informar o e-mail")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string email { get; set; }
    [Required(ErrorMessage = "É necessário informar a senha")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    [MaxLength(25, ErrorMessage = "A senha é muito grande")]
    [JsonPropertyName("password")]
    public string Password { get; set; }
}