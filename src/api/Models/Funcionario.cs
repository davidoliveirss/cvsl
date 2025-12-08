using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

// Representation of the SQL table `funcionarios` (from veterinarios.sql)

[Table("funcionarios")]
public class Funcionario
{
    [Key]
    public int Id { get; set; }

    // NOT NULL in SQL
    public string Nome { get; set; } = string.Empty;

    // Optional
    public string? Especialidade { get; set; }

    // NOT NULL in SQL
    public string Telefone { get; set; } = string.Empty;

    // NOT NULL in SQL
    public string Email { get; set; } = string.Empty;

    // NOT NULL in SQL
    public string Password { get; set; } = string.Empty;

    // NOT NULL in SQL
    public decimal Salario { get; set; }

    // Foreign key to Clinicas(id)
    [Column("id_clinica")]
    public int ClinicaId { get; set; }
}
