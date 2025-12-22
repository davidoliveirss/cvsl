using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

[Table("produtos")]
public class Produto
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("nome")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [Column("id_categoria")]
    public int IdCategoria { get; set; }

    [ForeignKey("IdCategoria")]
    public Categoria? Categoria { get; set; }

    [Required]
    [Column("preco")]
    public decimal Preco { get; set; }

    [Required]
    [Column("unidades_por_caixa")]
    public int UnidadesPorCaixa { get; set; }

    [Required]
    [Column("quantidade_stock")]
    public int QuantidadeStock { get; set; } = 0;

    [Required]
    [Column("id_clinica")]
    public int IdClinica { get; set; }

    [ForeignKey("IdClinica")]
    public Clinica? Clinica { get; set; }
}
