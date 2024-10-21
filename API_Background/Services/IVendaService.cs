using API_Background.Models;

namespace API_Background.Services
{
    public interface IVendaService
    {
        Task CriarVendaAsync(Venda venda);
        Task<IEnumerable<Venda>> ObterTodasVendas();
        Task<IEnumerable<Venda>> ObterVendasValidasProcessadas();
        Task<IEnumerable<Venda>> ObterVendasValidasNaoProcessadas();
        Task ProcessarVendasAsync();
    }
}
