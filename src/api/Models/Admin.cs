using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("admins")]
public class Admin
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("nome")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Column("nivel")]
    [MaxLength(20)]
    public string Nivel { get; set; } = "admin";

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
