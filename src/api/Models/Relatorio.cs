// Models/Relatorio.cs
namespace api.Models
{
    public class Relatorio
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string DiagnosticoPresuntivo { get; set; } = string.Empty;
        public string? DiagnosticoDefinitivo { get; set; }
        public string Observacoes { get; set; } = string.Empty;
        public DateOnly DataConsulta { get; set; }
        public int IdAnimal { get; set; }
        public int IdClinica { get; set; }
        public bool Ativo { get; set; } = true;  // Recomendado para soft delete
    }
}
