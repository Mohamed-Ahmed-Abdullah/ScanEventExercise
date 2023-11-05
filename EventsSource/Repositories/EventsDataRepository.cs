using DataModels.API;
using DataModels.JsonConverters;
using Newtonsoft.Json;

namespace EventsAPI.Repositories
{
    //This is a fake representation of the original source of the scan events.
    public class EventsDataRepository
    {
        private static readonly DateTime _startTime = DateTime.Now;

        public EventCollection? GetEvents(ulong fromEventId, int limit)
        {
            var numberOfRecords = (int)(DateTime.Now - _startTime).TotalSeconds / 10;
            Console.WriteLine("numberOfRecords=" + numberOfRecords);

            if (numberOfRecords < 1)
                return null;

            //TODO: this should be in config file.
            string jsonFilePath = "Repositories/FakeScanEvents.json";
            string jsonData = File.ReadAllText(jsonFilePath);
            var eventCollection = JsonConvert.DeserializeObject<EventCollection>(jsonData, new JsonSerializerSettings { Converters = { new EventTypeJsonConverter() } });

            return new EventCollection
            {
                //Add new ecent every 10 second
                ScanEvents = eventCollection.ScanEvents
                .Take(numberOfRecords)
                //.Select(index =>
                //{
                //    return new ScanEvent
                //    {
                //        EventId = (ulong)(83269 + index),
                //        ParcelId = (ulong)(5002 + index),
                //        Type = (EventType)new Random().Next(3),
                //        CreatedDateTimeUtc = DateTime.UtcNow,
                //        StatusCode = "",
                //        Device = new Device
                //        {
                //            DeviceTransactionId = (ulong)(83269 + index),
                //            DeviceId = (ulong)(103 + index)
                //        },
                //        User = new User
                //        {
                //            UserId = "NC1001",
                //            CarrierId = (CarrierCodes)new Random().Next(4),
                //            RunId = "100"
                //        }
                //    };
                //})
                .Where(w => w.EventId > fromEventId)
                .Take(limit)
                .ToArray()
            };
        }


    }
}
