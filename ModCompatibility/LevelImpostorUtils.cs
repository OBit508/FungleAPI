using BepInEx;
using BepInEx.Unity.IL2CPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility
{
    public static class LevelImpostorUtils
    {
        public static Assembly GetLevelImpostor()
        {
            PluginInfo info;
            IL2CPPChainloader.Instance.Plugins.TryGetValue("com.DigiWorm.LevelImposter", out info);
            if (info != null)
            {
                return info.Instance?.GetType().Assembly;
            }
            return null;
        }
    }
}
