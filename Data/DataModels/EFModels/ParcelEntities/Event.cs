using DataModels.API;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels.EFModels.ParcelEntities
{
    public class Event
    {
        [Key]
        public ulong EventId { get; set; }

        [ForeignKey("Parcel")]
        public ulong ParcelId { get; set; }
        
        public EventType Type { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }
        public ulong? DeviceTransactionId { get; set; }
        public ulong? DeviceId { get; set; }
        public string? UserId { get; set; }
        public CarrierCodes? CarrierId { get; set; }
        public string RunId { get; set; }
    }
}
