namespace DataModels.API
{
    public class ScanEvent
    {
        public ulong EventId { get; set; }
        public ulong ParcelId { get; set; }

        public EventType Type { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
        public Device Device { get; set; }
        public User User { get; set; }
    }
}
