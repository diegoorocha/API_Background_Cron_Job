using API_Background.Services;

namespace API_Background.BackgroundServices
{
    public class VendaBackgroundService(IServiceProvider serviceProvider) : IHostedService, IDisposable
    {
        private Timer _tempo;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private CancellationTokenSource _cancellationTokenSource; // Para gerenciar o cancelamento

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Cria um CancellationTokenSource para controlar a execução do tempo
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Configura o tempo para chamar o método a cada 3 minutos
            _tempo = new Timer(ExecutarProcessamentoVendas, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tempo?.Change(Timeout.Infinite, 0); // Para o tempo
            _cancellationTokenSource.Cancel(); // Solicita o cancelamento
            return Task.CompletedTask;
        }

        private void ExecutarProcessamentoVendas(object state)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            // Cria um escopo para a execução
            var scope = _serviceProvider.CreateScope();
            var vendaService = scope.ServiceProvider.GetRequiredService<IVendaService>();

            // Executa a tarefa fora do 'using', garantindo que o escopo permaneça ativo
            Task.Run(async () =>
            {
                try
                {
                    await vendaService.ProcessarVendasAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar vendas: {ex.Message}");
                }
                finally
                {
                    // Garante que o escopo seja descartado após a execução completa
                    scope.Dispose();
                }
            });
        }

        public void Dispose()
        {
            _tempo?.Dispose(); 
            _cancellationTokenSource?.Dispose();
        }
    }
}
