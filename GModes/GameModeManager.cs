using BepInEx.Configuration;
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using FungleAPI.Extensions;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Options;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace FungleAPI.GModes
{
    public static class GameModeManager
    {
        public static NormalGameMode Default = new NormalGameMode();
        public static Dictionary<uint, BaseGameMode> GameModes = new Dictionary<uint, BaseGameMode>();

        private static List<StringNames> Values = new List<StringNames>();
        private static StringGameSetting Data;
        internal static ConfigEntry<uint> HostValue;
        internal static uint NonHostValue;

        public static BaseGameMode GetCurrentGameMode() 
        {
            if (GameModes.TryGetValue(AmongUsClient.Instance.AmHost ? HostValue.Value : NonHostValue, out BaseGameMode baseGameMode))
            {
                return baseGameMode;
            }
            return Default;
        }

        public static void RegisterGameMode(Type type, ModPlugin modPlugin)
        {
            BaseGameMode gameMode = (BaseGameMode)Activator.CreateInstance(type);
            gameMode.GameModeId = (uint)Values.Count;
            Values.Add(gameMode.GameModeName);
            if (Data != null)
            {
                Data.Values = Values.ToArray();
            }
            gameMode.Initialize(modPlugin);
            GameModes.Add(gameMode.GameModeId, gameMode);
            modPlugin.BasePlugin.Log.LogInfo("Registered GameMode " + type.Name + " Id: " + gameMode.GameModeId.ToString());
        }

        public static OptionBehaviour CreateGameModeOption(Transform parent)
        {
            StringGameSetting stringGameSetting = Data.SafeCast<StringGameSetting>();
            StringOption stringOption = OptionManager.CreateEnumOption(parent, stringGameSetting, delegate (StringOption stringOption)
            {
                bool changed = HostValue.Value != stringOption.Value;

                HostValue.Value = (uint)stringOption.Value;
                stringGameSetting.Index = stringOption.Value;

                if (changed)
                {
                    SyncManager.RpcSyncGamemode();
                }
            });
            stringGameSetting.Index = (int)HostValue.Value;
            stringOption.Value = (int)HostValue.Value;
            return stringOption;
        }

        [EventRegister]
        public static void Initialize(FinishedPluginLoadingEvent finishedPluginLoadingEvent)
        {
            Data = ScriptableObject.CreateInstance<StringGameSetting>().DontUnload();
            StringGameSetting stringGameSetting = (StringGameSetting)Data;
            stringGameSetting.Type = OptionTypes.String;
            stringGameSetting.Title = FungleTranslation.GameModeText;
            stringGameSetting.Values = Values.ToArray();

            HostValue = FungleApiPlugin.Instance.Config.Bind("Essential", "CurrentGamemode", (uint)0);

            if (Values.Count > HostValue.Value)
            {
                HostValue.Value = 0;
            }

            stringGameSetting.Index = (int)HostValue.Value;
        }
    }
}
