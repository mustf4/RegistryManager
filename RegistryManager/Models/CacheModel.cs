using System;

namespace RegistryManager
{
    internal class CacheModel
    {
        public object Value { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public CacheModel(object value) => Value = value;
    }
}
