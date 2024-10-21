using System.ComponentModel.DataAnnotations;

namespace API_Background.Models
{
    public class CarroVenda
    {
        [Key]
        public int Id { get; set; }
        public int CarroId { get; set; }
        public int VendaId { get; set; }

        // Relacionamentos
        public virtual Carro? Carro { get; set; }
        public virtual Venda? Venda { get; set; }
    }
}
