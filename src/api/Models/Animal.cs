using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("animais")]
public class Animal
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("transponder")]
    [MaxLength(15)]
    public string? Transponder { get; set; }

    [Required]
    [Column("nome")]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [Column("especie")]
    [MaxLength(50)]
    public string? Especie { get; set; }

    [Column("raca")]
    [MaxLength(50)]
    public string? Raca { get; set; }

    [Column("data_nascimento")]
    public DateOnly? DataNascimento { get; set; }

    [Column("sexo")]
    [MaxLength(1)]
    public string? Sexo { get; set; }

    [Required]
    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [ForeignKey("IdCliente")]
    public Cliente? Cliente { get; set; }

    [Required]
    [Column("id_clinica")]
    public int IdClinica { get; set; }

    [ForeignKey("IdClinica")]
    public Clinica? Clinica { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
