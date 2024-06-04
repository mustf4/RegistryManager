using Microsoft.Win32;
using RegistryManager.Common.Models;
using System;

namespace RegistryManager.Common
{
    public class LocalMachine : BaseRegistry
    {
        protected override RegistryHive GetRegistryHive()
        {
            return RegistryHive.LocalMachine;
        }

        public Configuration ManagerConfiguration
        {
            get => GetDeserializedRegistryValue(RegSubKey.RegistryManager, RegName.ManagerConfiguration, new Configuration(), true, true);
            set => SetSerializedRegistryValue(RegSubKey.RegistryManager, RegName.ManagerConfiguration, value, true);
        }

        public Configuration ManagerConfigurationNotCached
        {
            get => GetDeserializedRegistryValue(RegSubKey.RegistryManager, RegName.ManagerConfiguration, new Configuration(), createIfNotExist: true);
            set => SetSerializedRegistryValue(RegSubKey.RegistryManager, RegName.ManagerConfiguration, value);
        }

        public Guid OSProductContentId
        {
            get => GetRegistryValue<Guid>(RegSubKey.ProductOptions, RegName.OSProductContentId);
            set => SetRegistryValue(RegSubKey.ProductOptions, RegName.OSProductContentId, value, RegistryValueKind.String);
        }

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
    }
}
