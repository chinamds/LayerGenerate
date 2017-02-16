using System;
using Microsoft.Win32;

namespace LayerGen
{
    public class RegistryFunctions
    {
        public static string RegValue(RegistryHive hive, string key, string valueName, ref string errInfo)
        {
            RegistryKey objParent = null;
            string sAns = "";
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    objParent = Registry.ClassesRoot;
                    break;
                case RegistryHive.CurrentConfig:
                    objParent = Registry.CurrentConfig;
                    break;
                case RegistryHive.CurrentUser:
                    objParent = Registry.CurrentUser;
                    break;
                case RegistryHive.DynData:
                    objParent = Registry.DynData;
                    break;
                case RegistryHive.LocalMachine:
                    objParent = Registry.LocalMachine;
                    break;
                case RegistryHive.PerformanceData:
                    objParent = Registry.PerformanceData;
                    break;
                case RegistryHive.Users:
                    objParent = Registry.Users;
                    break;
            }

            try
            {
                if (objParent == null)
                    return "";
                RegistryKey objSubkey = objParent.OpenSubKey(key);
                //if can't be found, object is not initialized
                if ((objSubkey != null))
                {
                    sAns = (string) (objSubkey.GetValue(valueName));
                }


            }
            catch (Exception ex)
            {
                errInfo = ex.Message;

            }
            finally
            {
                //if no error but value is empty, populate errinfo
                if (string.IsNullOrEmpty(errInfo) & string.IsNullOrEmpty(sAns))
                {
                    errInfo = "No value found for requested registry key";
                }
            }
            return sAns;
        }

        public static bool WriteToRegistry(RegistryHive parentKeyHive, string subKeyName, string valueName, object value)
        {
            RegistryKey objParentKey = null;
            bool bAns;

            try
            {
                switch (parentKeyHive)
                {
                    case RegistryHive.ClassesRoot:
                        objParentKey = Registry.ClassesRoot;
                        break;
                    case RegistryHive.CurrentConfig:
                        objParentKey = Registry.CurrentConfig;
                        break;
                    case RegistryHive.CurrentUser:
                        objParentKey = Registry.CurrentUser;
                        break;
                    case RegistryHive.DynData:
                        objParentKey = Registry.DynData;
                        break;
                    case RegistryHive.LocalMachine:
                        objParentKey = Registry.LocalMachine;
                        break;
                    case RegistryHive.PerformanceData:
                        objParentKey = Registry.PerformanceData;
                        break;
                    case RegistryHive.Users:
                        objParentKey = Registry.Users;

                        break;
                }

                if (objParentKey == null)
                    return false;

                RegistryKey objSubKey = objParentKey.OpenSubKey(subKeyName, true) ??
                                        objParentKey.CreateSubKey(subKeyName);

                if (objSubKey == null)
                    return false;
                objSubKey.SetValue(valueName, value);
                bAns = true;
            }
            catch (Exception)
            {
                bAns = false;

            }

            return bAns;
        }
    }
}
