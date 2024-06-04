using BenchmarkDotNet.Attributes;
using RegistryManager.Common;
using RegistryManager.Common.Models;
using System;

namespace RegistryManager.Benchmark
{
    public class Benchmarks
    {
        [Params(true, false)]
        public bool Cached { get; set; }
        public static Configuration _configuration = new Configuration { Id = 1, IsActive = true, LastModification = DateTime.Now, Value = "This is test value for the Registry Manager configuration, which should be stored in the registry." };

        [Benchmark]
        public void SerializeAndSave()
        {
            if (Cached)
                Reg.LocalMachine.ManagerConfiguration = _configuration;
            else
                Reg.LocalMachine.ManagerConfigurationNotCached = _configuration;
        }

        [Benchmark]
        public void DeserializeAndGet()
        {
            Configuration configuration = Cached ? Reg.LocalMachine.ManagerConfiguration : Reg.LocalMachine.ManagerConfigurationNotCached;
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {

        }
    }
}
