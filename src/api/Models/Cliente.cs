using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("clientes")]
public class Cliente
{
    [Key]
    public int Id { get; set; }

    // NOT NULL in SQL
    public string Nome { get; set; } = string.Empty;

    // NIF is optional (VARCHAR(9))
    public string? Nif { get; set; }

    // Morada is optional (VARCHAR(200))
    public string? Morada { get; set; }

    // NOT NULL in SQL
    public string Telefone { get; set; } = string.Empty;

    // Optional email
    public string? Email { get; set; }

    // Foreign key to Clinicas(id) - required in SQL
    // Use `ClinicaId` to follow common EF conventions.
    [Column("id_clinica")]
    public int ClinicaId { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
