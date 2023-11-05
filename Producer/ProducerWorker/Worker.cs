using ProducerWorker.API;

namespace ProducerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPullEvents _pullEvents;

        public Worker(ILogger<Worker> logger,
                      IPullEvents pullEvents
                      )
        {
            _logger = logger;
            _pullEvents = pullEvents;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    _pullEvents.Pull();
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, ex?.Message);
                }

                // Ideally the delay seconds should be in the config file.
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}