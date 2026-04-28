using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.PluginLoading
{
    public static class FunglePlugin<T> where T : BasePlugin
    {
        private static ModPlugin __plugin;
        public static T Instance => (T)Plugin.BasePlugin;
        public static ManualLogSource Logger => Instance.Log;
        public static ModPlugin Plugin
        {
            get
            {
                if (__plugin == null)
                {
                    __plugin = ModPluginManager.AllPlugins.FirstOrDefault(m => m.BasePlugin.GetType() == typeof(T));
                }
                return __plugin;
            }
        }

    }
}
