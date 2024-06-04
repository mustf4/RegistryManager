using System;

namespace RegistryManager.Common.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime LastModification { get; set; }
        public bool IsActive { get; set; }
    }
}
