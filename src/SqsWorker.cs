using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TesteECS.Services;

namespace TesteECS
{
    public class SqsWorker : BackgroundService
    {
        readonly ILogger<SqsWorker> _logger;
        readonly IServiceProvider _serviceProvider;
        private string[] queueUrls;

        public SqsWorker(IServiceProvider serviceProvider, string queueUrl, ILogger<SqsWorker> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ISqsService>();
                queueUrls = (await service.ListQueues(cancellationToken)).ToArray();
            }
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(queueUrls.Select(_ => ReceiveMessage(_, stoppingToken)));
        }

        private async Task ReceiveMessage(string queueUrl, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ISqsService>();
                    // _logger.LogInformation($"Obtendo mensagem de {queueUrl}: {DateTimeOffset.Now}");
                    var message = await service.ReceiveMessageQueue(queueUrl, stoppingToken, 5);
                    if (!string.IsNullOrEmpty(message)) _logger.LogInformation($"Mensagem em {queueUrl}: {message}");
                    // else _logger.LogInformation($"Nenhuma mensagem em {queueUrl}");
                }
            }
        }
    }
}