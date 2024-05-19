using RegistryManager;
using System;

namespace MultitaskScheduler.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool platformSupportMiracast = Reg.LocalMachine.PlatformSupportMiracast;
            string productName = Reg.LocalMachine.ProductName;
            Guid oSProductContentId = Reg.LocalMachine.OSProductContentId;
        }
    }
}
