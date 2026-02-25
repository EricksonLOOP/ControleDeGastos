using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CGD.APP.DTOs.Group;

public class UpdateGroupDto
{
    [JsonPropertyName("name")]
    [Required]
    public string Name { get; set; }
}