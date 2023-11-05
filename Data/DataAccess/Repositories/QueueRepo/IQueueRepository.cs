using DataModels.API;
using DataModels.EFModels.QueueEntities;

namespace DataAccess.Repositories.QueueRepo
{
    public interface IQueueRepository
    {
        void PushEvents(EventCollection eventCollection);
        int GetCursor();
        void SetCursor(ulong cursor);
        EventQueue PopEvent();
        void DeleteEvent(ulong eventId);
    }
}
