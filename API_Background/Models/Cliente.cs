using System.ComponentModel.DataAnnotations;

namespace API_Background.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Relacionamentos
        public virtual ICollection<Venda> Vendas { get; set; } = [];
    }
}
