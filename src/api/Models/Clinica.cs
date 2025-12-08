using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("clinicas")]
public class Clinica
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    public string Cp { get; set; } = string.Empty;

    [MaxLength(9)]
    public string? Nif { get; set; }

    [Required]
    [MaxLength(25)]
    public string Iban { get; set; } = string.Empty;
}
