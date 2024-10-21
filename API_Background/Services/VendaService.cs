using API_Background.Exceptions;
using API_Background.Helpers;
using API_Background.Models;
using API_Background.Repositories;

namespace API_Background.Services
{
    public class VendaService(IBaseRepository<Venda> vendaRepository,
                            IBaseRepository<Cliente> clienteRepository,
                            IBaseRepository<CarroVenda> carroVendaRepository,
                            IBaseRepository<LogVendaProcessado> logVendaProcessadoRepository,
                            IBaseRepository<Carro> carroRepository) : IVendaService
    {
        private readonly IBaseRepository<Venda> _vendaRepository = vendaRepository;
        private readonly IBaseRepository<Cliente> _clienteRepository = clienteRepository;
        private readonly IBaseRepository<CarroVenda> _carroVendaRepository = carroVendaRepository;
        private readonly IBaseRepository<LogVendaProcessado> _logVendaProcessadoRepository = logVendaProcessadoRepository;
        private readonly IBaseRepository<Carro> _carroRepository = carroRepository;

        public async Task CriarVendaAsync(Venda venda)
        {
            ValidarInputVenda(venda);

            await ValidarDadosInputVenda(venda);

            await _vendaRepository.AdicionarAsync(venda);
        }

        public async Task<IEnumerable<Venda>> ObterTodasVendas()
        {
            return await _vendaRepository.ObterTodosAsync();
        }

        public async Task<IEnumerable<Venda>> ObterVendasValidasProcessadas()
        {
            return await _vendaRepository.ObterTodosAsync(v => v.VendaValida == VendaValida.Sim && v.Processado == true);
        }

        public async Task<IEnumerable<Venda>> ObterVendasValidasNaoProcessadas()
        {
            return await _vendaRepository.ObterTodosAsync(v => v.VendaValida == VendaValida.Nao && v.Processado == false);
        }

        public async Task ProcessarVendasAsync()
        {
            var vendasNaoProcessadas = await _vendaRepository.ObterTodosAsync(v => v.Processado == false);

            if (!vendasNaoProcessadas.Any())
                return;
            
            var (vendasValidas, vendasNaoValidas) = ValidarDadosProcessamento(vendasNaoProcessadas);

            foreach (var venda in vendasValidas)
            {
                venda.Processado = true; 
                venda.VendaValida = VendaValida.Sim;
            }

            await _vendaRepository.AtualizarEmLoteAsync(vendasValidas);
            await CriarLogVendaProcessamento(vendasValidas, vendasNaoValidas);
        }

        private async Task ValidarDadosInputVenda(Venda venda)
        {
            if (venda is { ClienteId: <= 0, CarrosVenda.Count: <= 0 })
                throw new ImputException("Venda Inválida !");

            if (await _clienteRepository.ObterAsync(c => c.Id == venda.ClienteId) is null)
                throw new ImputException("Cliente Inválido !");

            foreach (var carro in venda.CarrosVenda)
            {
                if (await _carroRepository.ObterAsync(c => c.Id == carro.CarroId) is null)
                {
                    throw new ImputException("Carro Inválido !");
                }
            }

            venda.VendaValida = VendaValida.Nao;
        }

        private static void ValidarInputVenda(Venda venda)
        {
            if (venda is null)
                throw new ImputException("Venda Inválida !");

            if (venda.ClienteId <= 0)
                throw new ImputException("Cliente Não Informado !");

            if (venda.CarrosVenda.Count <= 0)
                throw new ImputException("Carro Não Informado !");
        }

        private static Tuple<List<Venda>, List<Venda>> ValidarDadosProcessamento(IEnumerable<Venda> vendas)
        {
            var dadosValidos = new List<Venda>();
            var dadosNaoValidos = new List<Venda>();

            foreach (var venda in vendas)
            {
                if (venda is { Cliente.Documento: not null })
                {
                    if (DocumentoHelpers.ValidarDocumentoCpfCnpj(venda.Cliente.Documento))
                    {
                        dadosValidos.Add(venda);
                    }
                    else
                    {
                        dadosNaoValidos.Add(venda);
                    }
                }
                else
                {
                    dadosNaoValidos.Add(venda);
                }
            }

            return new Tuple<List<Venda>, List<Venda>>(dadosValidos, dadosNaoValidos);
        }

        private async Task CriarLogVendaProcessamento(List<Venda> vendasValidas, List<Venda> vendasNaoValidas)
        {
            var objetoLog = CriarObjetoLogVendaProcessamento(vendasValidas, vendasNaoValidas);

            await _logVendaProcessadoRepository.AtualizarEmLoteAsync(objetoLog);
        }

        private static List<LogVendaProcessado> CriarObjetoLogVendaProcessamento(List<Venda> vendasValidas, List<Venda> vendasNaoValidas)
        {
            var logs = new List<LogVendaProcessado>();

            foreach (var logValido in vendasValidas)
            {
                var log = new LogVendaProcessado() { VendaId = logValido.Id, DataProcessamento = DateTime.Now, Motivo = MotivoLogProcessado(logValido.Cliente!.Documento) };

                logs.Add(log);
            }

            foreach (var logNaoValido in vendasNaoValidas)
            {
                var log = new LogVendaProcessado() { VendaId = logNaoValido.Id, DataProcessamento = DateTime.Now, Motivo = MotivoLogNaoProcessado(logNaoValido.Cliente!.Documento) };

                logs.Add(log);
            }

            return logs;
        }

        private static string MotivoLogProcessado(string documento)
        {
            return $"O documento {documento} informado está valido !";
        }

        private static string MotivoLogNaoProcessado(string documento)
        {
            documento = documento.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

            return documento.Length == 11 ? "CPF Inválido !" :
                documento.Length == 14 ? "CNPJ Inválido !" : "Sem documento válido!";
        }
    }
}
