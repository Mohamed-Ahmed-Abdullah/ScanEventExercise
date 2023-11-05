using DataModels.API;

namespace ProducerWorker.Utilities
{
    public interface IJsonParser
    {
        ulong GetLastId(EventCollection eventCollection);
    }
}
