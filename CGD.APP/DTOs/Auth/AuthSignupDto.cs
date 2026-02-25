using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.Auth;

public class AuthSignupDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Username é obrigatório")]
    [MinLength(3, ErrorMessage = "Username deve ter no mínimo 3 caracteres")]
    [MaxLength(200, ErrorMessage = "Username deve ter no máximo 200 caracteres")]
    public string Name { get; set; }

    [JsonPropertyName("birthDate")]
    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    [Required(ErrorMessage = "Password é obrigatório")]
    [MinLength(8, ErrorMessage = "Password deve ter no mínimo 8 caracteres")]
    [MaxLength(25, ErrorMessage = "A senha é muito longa")]
    public string Password { get; set; }

    [JsonPropertyName("confirmPassword")]
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; }


}