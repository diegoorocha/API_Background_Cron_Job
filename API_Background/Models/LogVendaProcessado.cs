using System.ComponentModel.DataAnnotations;

namespace API_Background.Models
{
    public class LogVendaProcessado
    {
        [Key]
        public int Id { get; set; }
        public required int VendaId { get; set; }
        public DateTime DataProcessamento { get; set; } = DateTime.Now;
        public string Motivo { get; set; } = string.Empty;

        // Relacionamentos
        public virtual Venda? Venda { get; set; }
    }
}
