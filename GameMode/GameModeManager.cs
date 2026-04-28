using BepInEx.Configuration;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameMode
{
    public static class GameModeManager
    {
        public static Dictionary<Type, CustomGameMode> GameModeInstances = new Dictionary<Type, CustomGameMode>();
        public static Dictionary<string, CustomGameMode> GameModes = new Dictionary<string, CustomGameMode>();
        public static CustomGameMode DefaultGameMode = new NormalGameMode();
        internal static int LastGameMode = int.MinValue;
        internal static ConfigEntry<string> localMode;
        internal static string onlineMode;
        public static void Initialize()
        {
            localMode = FungleAPIPlugin.Instance.Config.Bind("GameMode", "LocalGameMode", typeof(NormalGameMode).GetShortUniqueId());
            if (GameModes.TryGetValue(localMode.Value, out CustomGameMode customGameMode))
            {
                DefaultGameMode = customGameMode;
            }
            else
            {
                DefaultGameMode = GameModeInstances[typeof(NormalGameMode)];
                localMode.Value = localMode.DefaultValue.ToString();
            }
            onlineMode = localMode.Value;
        }
        public static void RegisterGameMode(Type type, ModPlugin plugin)
        {
            LastGameMode++;
            CustomGameMode customGameMode = (CustomGameMode)Activator.CreateInstance(type);
            customGameMode.GameModeId = LastGameMode;
            plugin.GameModes.Add(customGameMode);
            GameModeInstances.Add(type, customGameMode);
            GameModes.Add(type.GetShortUniqueId(), customGameMode);
            customGameMode.OptionCollection = new GameOptions.Collections.DefaultOptionCollection();
            customGameMode.OptionCollection.Initialize(type, plugin);
        }
        public static CustomGameMode GetActiveGameMode()
        {
            try
            {
                if (AmongUsClient.Instance != null && GameModes.TryGetValue(AmongUsClient.Instance.AmHost ? localMode.Value : onlineMode, out CustomGameMode customGameMode))
                {
                    return customGameMode;
                }
                return DefaultGameMode;
            }
            catch { return DefaultGameMode; }
        }
    }
}
