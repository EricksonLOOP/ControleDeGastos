using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CGD.Domain.Entities;

[Table("users")] // recomendo minúsculo para Postgres
public class User

{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    // Age e dado derivado de BirthDate, persistido para consultas/ordenacoes rapidas.
    public int Age { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GroupMember> GroupMembers { get; set; }
}