using System.ComponentModel.DataAnnotations;

namespace DataModels.EFModels.ParcelEntities
{
    public class Parcel
    {
        [Key]
        public ulong ParcelId { get; set; }
        public string? StatusCode { get; set; }
        public DateTime? PickedUpDateTime { get; set; }
        public DateTime? DeliveredDateTime { get; set; }

        public List<Event>? Events { get; set; }
    }
}
