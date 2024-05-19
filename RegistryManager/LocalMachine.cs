using Microsoft.Win32;
using System;

namespace RegistryManager
{
    public class LocalMachine : BaseRegistry
    {
        public bool PlatformSupportMiracast
        { 
            get => GetRegistryValue(RegSubKey.GraphicsDriver, RegName.PlatformSupportMiracast, false, true, false);
            set => SetRegistryValue(RegSubKey.GraphicsDriver, RegName.PlatformSupportMiracast, value, RegistryValueKind.DWord, true);
        }

        public string ProductName
        {
            get => GetRegistryValue(RegSubKey.WindowsNtCurrentVersion, RegName.ProductName, "Windows ...", true, false);
            set => SetRegistryValue(RegSubKey.WindowsNtCurrentVersion, RegName.ProductName, value, RegistryValueKind.String, true);
        }

        public Guid OSProductContentId
        {
            get => GetRegistryValue<Guid>(RegSubKey.ProductOptions, RegName.OSProductContentId);
            set => SetRegistryValue(RegSubKey.ProductOptions, RegName.OSProductContentId, value, RegistryValueKind.String);
        }

        protected override RegistryHive GetRegistryHive()
        {
            return RegistryHive.LocalMachine;
        }
    }
}
