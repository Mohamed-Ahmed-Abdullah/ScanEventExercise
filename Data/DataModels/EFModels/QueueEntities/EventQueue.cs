using System.ComponentModel.DataAnnotations;

namespace DataModels.EFModels.QueueEntities
{
    public class EventQueue
    {
        [Key]
        public ulong EventId { get; set; }
        public string Payload { get; set; }

        //To determine the timeout for unprocessed items 
        public DateTime? PopDatetime { get; set; }

        //If the consumer tried many times to process the event and failed many times, we should move the event to "failed to process queue"
        public int ProcessAttempts { get; set; }

        public bool MaxRetriesReached { get; set; }
    }
}
