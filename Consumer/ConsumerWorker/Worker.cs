using DataAccess.Repositories.ParcelRepo;
using DataAccess.Repositories.QueueRepo;

namespace ConsumerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueRepository _queueRepository;
        private readonly IParcelRepository _parcelRepository;

        public Worker(ILogger<Worker> logger,
                      IQueueRepository queueRepository,
                      IParcelRepository parcelRepository)
        {
            _logger = logger;
            _queueRepository = queueRepository;
            _parcelRepository = parcelRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, stoppingToken);


                    //Pop Event
                    var scanEvent = _queueRepository.PopEvent();

                    //This is to test what will happen if the event was picked up but never processed.
                    if (scanEvent.EventId == 83274)
                        continue;

                    _logger.LogInformation($"Poped event: {scanEvent.EventId}");

                    //Process it
                    _parcelRepository.ProcessEvent(scanEvent);

                    //Delete it from the queue
                    _queueRepository.DeleteEvent(scanEvent.EventId);


                    _logger.LogInformation($"ConsumerWorker running at: {DateTimeOffset.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex?.Message);

                }
            }
        }
    }
}