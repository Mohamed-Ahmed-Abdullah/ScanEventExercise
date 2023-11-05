using DataModels.EFModels.QueueEntities;

namespace DataAccess.Repositories.ParcelRepo
{
    public interface IParcelRepository
    {
        void ProcessEvent(EventQueue @event);
    }
}
