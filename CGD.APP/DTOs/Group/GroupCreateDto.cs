using System.ComponentModel.DataAnnotations;

namespace CGD.APP.DTOs.Group;

public class GroupCreateDto
{
    [Required]
    public string Name { get; set; }
}
