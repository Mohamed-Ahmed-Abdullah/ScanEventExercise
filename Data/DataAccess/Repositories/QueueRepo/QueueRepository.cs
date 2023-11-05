using DataModels.API;
using DataModels.EFModels.QueueEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataAccess.Repositories.QueueRepo
{
    public class QueueRepository : IQueueRepository
    {
        #region Constructor
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<QueueRepository> _logger;
        private readonly IConfiguration _configuration;

        public QueueRepository(ILogger<QueueRepository> logger,
                               IServiceScopeFactory scopeFactory,
                               IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }
        #endregion

        #region Public methods
        public void PushEvents(EventCollection eventCollection)
        {
            try
            {
                // Unforetunately SQL Lite doesn't support transactions, otherwise we should be using it here.
                using (var scope = _scopeFactory.CreateScope())
                using (var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>())
                {
                    if (eventCollection == null)
                        return;

                    foreach (var scanEvent in eventCollection.ScanEvents)
                    {
                        AddIfNotExists(dbContext, scanEvent);
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex?.Message);
            }
        }

        public EventQueue PopEvent()
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>())
            {
                var WorkerTimeout = TimeSpan.Parse(_configuration["WorkerTimeout"]);

                //Sql light can't translate the datetime query, I'm forced to get the full table and query it on the client side. this is very bad and not ideal.
                var scanEvent = dbContext.EventQueue
                                .ToList()
                                .Where(w => !w.MaxRetriesReached && (w.PopDatetime == null || DateTime.Now >= (w.PopDatetime + WorkerTimeout ))  )
                                .FirstOrDefault();

                if (scanEvent == null)
                    throw new Exception("Queue is Empty.");

                if (scanEvent.ProcessAttempts >= int.Parse(_configuration["MaxNumberOfAttempts"]) - 1)
                    scanEvent.MaxRetriesReached = true;

                scanEvent.PopDatetime = DateTime.Now;
                scanEvent.ProcessAttempts += 1;
                dbContext.SaveChanges();

                return scanEvent;
            }
        }

        public void DeleteEvent(ulong eventId)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>())
            {
                var scanEvent = dbContext.EventQueue.FirstOrDefault(f => f.EventId == eventId);

                if (scanEvent == null)
                    return;

                dbContext.EventQueue.Remove(scanEvent);
                dbContext.SaveChanges();
            }
        }

        public int GetCursor()
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>())
            {
                var cursor = dbContext.Settings.FirstOrDefault(f => f.Key == "Cursor");
                if (cursor != null)
                    return int.Parse(cursor.Value);

                _logger.LogError("can't get cursor position from the db.");
                throw new Exception("can't get cursor position from the db.");
            }
        }

        public void SetCursor(ulong cursor)
        {
            using (var scope = _scopeFactory.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<QueueDbContext>())
            {
                dbContext.Settings.Update(new Settings { Key = "Cursor", Value = $"{cursor}" });
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region Private methods
        private static void AddIfNotExists(QueueDbContext dbContext, ScanEvent scanEvent)
        {
            //This Query is bad as it bringns the whole row back, we are only interested in the EventId, but for the sake of time I will leave it.
            var existingEvent = dbContext.EventQueue
                                    .FirstOrDefault(e => e.EventId == scanEvent.EventId);

            if (existingEvent == null)
            {
                dbContext.EventQueue.Add(new EventQueue
                {
                    EventId = scanEvent.EventId,
                    Payload = JsonConvert.SerializeObject(scanEvent)
                });
            }
        }
        #endregion
    }
}
