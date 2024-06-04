using RegistryManager.Common;
using RegistryManager.Common.Models;
using System;

namespace RegistryManager.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool platformSupportMiracast = Reg.LocalMachine.PlatformSupportMiracast;
            string productName = Reg.LocalMachine.ProductName;
            Guid oSProductContentId = Reg.LocalMachine.OSProductContentId;
            Configuration configuration = Reg.LocalMachine.ManagerConfiguration;
        }
    }
}
