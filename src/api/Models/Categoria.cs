using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("categorias")]
public class Categoria
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("nome")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    [MaxLength(500)]
    public string? Descricao { get; set; }

    [Required]
    [Column("id_clinica")]
    public int IdClinica { get; set; }

    [ForeignKey("IdClinica")]
    public Clinica? Clinica { get; set; }
}
