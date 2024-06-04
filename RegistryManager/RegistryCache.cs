using MultitaskScheduler;
using System;
using System.Collections.Concurrent;

namespace RegistryManager
{
    internal static class RegistryCache
    {
        private static readonly int _timeoutMinutes = 60;
        private static readonly Scheduler _scheduler = Scheduler.Factory.CreateNew("Registry Scheduler");
        private static readonly ConcurrentDictionary<string, CacheModel> _values = new ConcurrentDictionary<string, CacheModel>();

        static RegistryCache()
        {
            _scheduler.ScheduleJob("Caching", 10, CleanupByTimeout);
        }

        public static void AddValue<T>(string key, T value)
        {
            _values.AddOrUpdate(key, new CacheModel(value), (s, v) =>
            {
                v.Value = value;
                return v;
            });
        }

        public static TR GetValue<TR>(string key)
        {
            if (_values.TryGetValue(key, out CacheModel cacheModel))
            {
                return (TR)cacheModel.Value;
            }

            return default;
        }

        public static bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        private static void CleanupByTimeout()
        {
            DateTime currentDate = DateTime.Now;

            foreach (var current in _values)
            {
                CacheModel cacheModel = current.Value;
                if (cacheModel.UpdatedDate < currentDate.AddMinutes(-_timeoutMinutes))
                    _values.TryRemove(current.Key, out _);
            }
        }
    }
}
