using System.ComponentModel.DataAnnotations;

namespace API_Background.Models
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public VendaValida VendaValida { get; set; }
        public bool Processado { get; set; } = false; 
        public int ClienteId { get; set; }

        // Relacionamentos
        public virtual Cliente? Cliente { get; set; }
        public virtual ICollection<CarroVenda> CarrosVenda { get; set; } = [];
        public virtual ICollection<LogVendaProcessado> LogsProcessamento { get; set; } = [];
    }

    public enum VendaValida
    {
        Nao = 0,
        Sim = 1
    }
}
