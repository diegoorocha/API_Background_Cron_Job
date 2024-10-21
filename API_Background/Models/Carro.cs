using System.ComponentModel.DataAnnotations;

namespace API_Background.Models
{
    public class Carro
    {
        [Key]
        public int Id { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public decimal Preco { get; set; } = decimal.Zero;

        // Relacionamentos
        public virtual ICollection<CarroVenda> CarrosVenda { get; set; } = [];
    }
}
