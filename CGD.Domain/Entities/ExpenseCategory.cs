using System.ComponentModel.DataAnnotations;

namespace CGD.Domain.Entities;

public class ExpenseCategory
{
    [Key]
    public Guid Id { get; set; }
    // Ownership da categoria: define qual usuario pode usar/editar este cadastro.
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(400)]
    public string Description { get; set; } = null!;
    // Purpose guia validacao de compatibilidade com o tipo da transacao.
    public CategoryPurpose Purpose { get; set; }
}
