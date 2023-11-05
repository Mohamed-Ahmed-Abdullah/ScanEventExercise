using System.ComponentModel.DataAnnotations;

namespace DataModels.EFModels.QueueEntities
{
    public class Settings
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
