using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

// Representation of the SQL table `funcionarios` (from veterinarios.sql)

[Table("funcionarios")]
public class Funcionario
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("especialidade")]
    public string? Especialidade { get; set; }

    [Column("telefone")]
    public string Telefone { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("salario")]
    public decimal Salario { get; set; }

    [Column("id_clinica")]
    public int ClinicaId { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    // Navigation property
    public Clinica? Clinica { get; set; }
}
