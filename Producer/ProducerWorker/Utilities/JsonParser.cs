using DataModels.API;

namespace ProducerWorker.Utilities
{
    internal class JsonParser : IJsonParser
    {
        public ulong GetLastId(EventCollection eventCollection)
        {
            if (eventCollection == null || 
                eventCollection.ScanEvents.Length == 0)
                throw new Exception("eventCollection doesn't have any events.");

            return eventCollection.ScanEvents.Select(s => s.EventId).Max();
        }
    }
}
