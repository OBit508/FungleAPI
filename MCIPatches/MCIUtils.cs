using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Configuration;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.MCIPatches
{
    public static class MCIUtils
    {
        public static Assembly GetMCI()
        {
            PluginInfo info;
            IL2CPPChainloader.Instance.Plugins.TryGetValue("dragonbreath.au.mci", out info);
            if (info != null)
            {
                return info.Instance?.GetType().Assembly;
            }
            return null;
        }
        public static string GetMCIVersion()
        {
            Assembly mci = GetMCI();
            if (mci != null)
            {
                PropertyInfo property = mci.GetType("MCI.MCIPlugin").GetProperty("Version");
                if (property != null && property.PropertyType == typeof(string))
                {
                    return (string)property.GetValue(null);
                }
            }
            return "";
        }
        public static ClientData GetClient(int clientId)
        {
            Assembly mci = GetMCI();
            if (mci != null)
            {
                FieldInfo field = mci.GetType("MCI.InstanceControl").GetField("Clients");
                if (field != null && field.GetValue(null).SimpleCast<Dictionary<int, ClientData>>() != null)
                {
                    ClientData client;
                    field.GetValue(null).SimpleCast<Dictionary<int, ClientData>>().TryGetValue(clientId, out client);
                    return client;
                }
            }
            return null;
        }
    }
}
