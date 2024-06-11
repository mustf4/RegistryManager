using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace RegistryManager
{
    public abstract partial class BaseRegistry
    {
        private static bool? _is64BitOperatingSystem = null;

        public static bool Is64BitOperatingSystem
        {
            get
            {
                if (!_is64BitOperatingSystem.HasValue)
                {
                    _is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
                }
                return _is64BitOperatingSystem.Value;
            }
        }

        protected abstract RegistryHive GetRegistryHive();

        protected TR GetRegistryValue<TR>(string subKey, string name, TR defaultValue = default, bool cache = false, bool createIfNotExist = false, RegistryValueKind valueKind = RegistryValueKind.String)
        {
            TR value = default;
            string cacheKey = GetCacheKey(subKey, name);

            try
            {
                if (cache && RegistryCache.ContainsKey(cacheKey))
                    return RegistryCache.GetValue<TR>(cacheKey);

                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(GetRegistryHive(), GetRegistryView()).OpenSubKey(subKey, createIfNotExist))
                {
                    object registryValue = null;

                    if (registryKey != null)
                    {
                        if (createIfNotExist)
                        {
                            registryValue = registryKey.GetValue(name, "-1");
                            if (string.Equals(registryValue.ToString(), "-1"))
                                registryValue = CreateRegistryValue(subKey, name, defaultValue, cache, valueKind);
                        }
                        else
                            registryValue = registryKey.GetValue(name, defaultValue);
                    }
                    else if (createIfNotExist)
                    {
                        registryValue = CreateRegistryValue(subKey, name, defaultValue, cache, valueKind);
                    }

                    value = registryValue == null ? defaultValue : ParseValue(registryValue, defaultValue);

                    if (cache)
                        RegistryCache.AddValue(cacheKey, value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{cacheKey}. {ex}");
            }

            return value;
        }

        private object CreateRegistryValue<TR>(string subKey, string name, TR defaultValue, bool cache, RegistryValueKind valueKind)
        {
            TR defaultForType = defaultValue;
            if (defaultValue == null)
                defaultForType = (TR)(object)string.Empty;

            SetRegistryValue(subKey, name, defaultForType, valueKind, cache);
            return defaultForType;
        }

        protected bool SetRegistryValue<T>(string subKey, string name, T value, RegistryValueKind valueKind = RegistryValueKind.DWord, bool cache = false)
        {
            bool result = false;
            string cacheKey = GetCacheKey(subKey, name);

            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(GetRegistryHive(), GetRegistryView()).CreateSubKey(subKey))
                {
                    if (registryKey != null)
                    {
                        registryKey.SetValue(name, GetValueToSet(value), valueKind);

                        if (cache)
                            RegistryCache.AddValue(cacheKey, value);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{cacheKey}. {ex}");
            }

            return result;
        }

        protected TR GetDeserializedRegistryValue<TR>(string subKey, string name, TR defaultValue = default, bool cache = false, bool createIfNotExist = false)
        {
            try
            {
                return JsonSerializer.Deserialize<TR>(GetRegistryValue(subKey, name, JsonSerializer.Serialize(defaultValue), cache, createIfNotExist));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error in: {GetCacheKey(subKey, name)}. {ex}");
                return (TR)(object)defaultValue;
            }
        }

        protected bool SetSerializedRegistryValue<T>(string subKey, string name, T value, bool cache = false)
        {
            try
            {
                return SetRegistryValue(subKey, name, JsonSerializer.Serialize(value), RegistryValueKind.String, cache);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Serialization error in: {GetCacheKey(subKey, name)}. {ex}");
                return false;
            }
        }

        private object GetValueToSet<T>(T value)
        {
            Type type = typeof(T);
            if (type == typeof(bool))
                return bool.Parse(value.ToString()) ? 1 : 0;

            return value;
        }

        private static string GetCacheKey(string subKey, string name)
        {
            return string.Concat(subKey.TrimEnd('\\'), "\\", name);
        }

        private TR ParseValue<TR>(object value, TR defaultValue)
        {
            TR result = defaultValue;

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return result;

            try
            {
                Type type = typeof(TR);

                switch (type)
                {
                    case Type _ when type == typeof(int):
                        if (int.TryParse(value.ToString(), out int iResult))
                            result = (TR)(object)iResult;
                        break;
                    case Type _ when type == typeof(double):
                        if (double.TryParse(value.ToString(), out double dResult))
                            result = (TR)(object)dResult;
                        break;
                    case Type _ when type == typeof(bool):
                        if (bool.TryParse(value.ToString(), out bool bResult))
                            result = (TR)(object)bResult;
                        else if (int.TryParse(value.ToString(), out int ibResult))
                            result = (TR)(object)(ibResult == 1);
                        break;
                    case Type _ when type == typeof(decimal):
                        if (decimal.TryParse(value.ToString(), out decimal nResult))
                            result = (TR)(object)nResult;
                        break;
                    case Type _ when type == typeof(DateTime):
                        if (DateTime.TryParse(value.ToString(), out DateTime dtResult))
                            result = (TR)(object)dtResult.ToUniversalTime();
                        else
                            result = (TR)(object)default(DateTime).ToUniversalTime();
                        break;
                    case Type _ when type == typeof(Guid):
                        if (Guid.TryParse(value.ToString(), out Guid guidResult))
                            result = (TR)(object)guidResult;
                        break;
                    case Type _ when type == typeof(List<string>):
                        result = (TR)(object)((string[])value).ToList();
                        break;
                    default:
                        result = (TR)Convert.ChangeType(value, typeof(TR));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        private static RegistryView GetRegistryView()
        {
            return Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
        }
    }
}
