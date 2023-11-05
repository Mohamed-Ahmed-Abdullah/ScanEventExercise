using DataModels.API;

namespace EventsAPI.Repositories
{
    public interface IEventsDataRepository
    {
        EventCollection GetEvents(ulong fromEventId, int limit);
    }
}
