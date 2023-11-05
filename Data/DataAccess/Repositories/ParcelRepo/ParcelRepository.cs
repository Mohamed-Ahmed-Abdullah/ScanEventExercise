using DataModels.API;
using DataModels.EFModels.ParcelEntities;
using DataModels.EFModels.QueueEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataAccess.Repositories.ParcelRepo
{
    public class ParcelRepository : IParcelRepository
    {
        private readonly ILogger<ParcelRepository> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ParcelRepository(ILogger<ParcelRepository> logger,
                                IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public void ProcessEvent(EventQueue @event)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                using (var dbContext = scope.ServiceProvider.GetRequiredService<ParcelDbContext>())
                {
                    var scanEvent = JsonConvert.DeserializeObject<ScanEvent>(@event.Payload);

                    if (scanEvent == null)
                    {
                        _logger.LogError($"Event Payload in the queue was empty, {@event.EventId}");
                        return;
                    }

                    var parcel = GetOrCreateParcel(dbContext, scanEvent.ParcelId);

                    parcel.Events ??= new List<Event>();

                    parcel.Events.Add(new Event
                    {
                        EventId = scanEvent.EventId,
                        ParcelId = parcel.ParcelId,
                        Type = scanEvent.Type,
                        CreatedDateTimeUtc = scanEvent.CreatedDateTimeUtc,
                        DeviceTransactionId = scanEvent.Device?.DeviceTransactionId,
                        DeviceId = scanEvent.Device?.DeviceId,
                        UserId = scanEvent.User?.UserId,
                        CarrierId = scanEvent.User?.CarrierId,
                        RunId = scanEvent.User?.RunId
                    });

                    if (scanEvent.Type == EventType.PICKUP)
                        parcel.PickedUpDateTime = scanEvent.CreatedDateTimeUtc;

                    if (scanEvent.Type == EventType.DELIVERY)
                        parcel.DeliveredDateTime = scanEvent.CreatedDateTimeUtc;

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex?.Message);
            }
        }

        private Parcel GetOrCreateParcel(ParcelDbContext dbContext, 
                                         ulong parcelId)
        {
            var parcel = dbContext.Parcels
                                  .Where(f => f.ParcelId == parcelId)
                                  .Include(i=>i.Events)
                                  .FirstOrDefault();
            if (parcel != null)
                return parcel;

            var newparcel = new Parcel { ParcelId = parcelId };
            dbContext.Parcels.Add(newparcel);
            return newparcel;
        }

    }
}
