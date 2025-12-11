using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("clinicas")]
public class Clinica
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    [Column("cp")]
    public string Cp { get; set; } = string.Empty;

    [MaxLength(9)]
    [Column("nif")]
    public string? Nif { get; set; }

    [Required]
    [MaxLength(25)]
    [Column("iban")]
    public string Iban { get; set; } = string.Empty;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
