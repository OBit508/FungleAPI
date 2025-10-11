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
        public static T Instance => Plugin.BasePlugin.SimpleCast<T>();
        public static ModPlugin Plugin => ModPlugin.AllPlugins.FirstOrDefault(m => m.BasePlugin.GetType() == typeof(T));
    }
}
