using AmongUs.GameOptions;
using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.Modifiers.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static Il2CppSystem.Globalization.CultureInfo;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace FungleAPI.Modifiers
{
    public static class ModifierManager
    {
        internal static uint LastModifierId = uint.MinValue;
        public static Dictionary<uint, BaseModifier> Modifiers = new Dictionary<uint, BaseModifier>();

        internal static Dictionary<PlayerControl, ModifierHolder> Holders = new Dictionary<PlayerControl, ModifierHolder>();

        public static T GetModifier<T>(this PlayerControl playerControl) where T : BaseModifier
        {
            return GetHolder(playerControl).Modifiers.OfType<T>().FirstOrDefault();
        }
        public static void RpcAddModifier(this PlayerControl playerControl, uint modifierId, bool sendLate = true)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            if (GetHolder(playerControl).AddModifier(modifierId))
            {
                if (sendLate)
                {
                    Rpc<RpcAddModifier>.Instance.SendLate((modifierId, playerControl), PlayerControl.LocalPlayer);
                    return;
                }
                Rpc<RpcAddModifier>.Instance.Send((modifierId, playerControl), PlayerControl.LocalPlayer);
            }
        }
        public static void RpcRemoveModifier(this PlayerControl playerControl, uint modifierId, bool sendLate = true)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            if (GetHolder(playerControl).RemoveModifier(modifierId))
            {
                if (sendLate)
                {
                    Rpc<RpcRemoveModifier>.Instance.SendLate((modifierId, playerControl), PlayerControl.LocalPlayer);
                    return;
                }
                Rpc<RpcRemoveModifier>.Instance.Send((modifierId, playerControl), PlayerControl.LocalPlayer);
            }
        }
        public static ModifierHolder GetHolder(PlayerControl playerControl)
        {
            if (Holders.TryGetValue(playerControl, out ModifierHolder modifierHolder))
            {
                return modifierHolder;
            }
            ModifierHolder holder = playerControl.GetComponent<ModifierHolder>();
            Holders[playerControl] = holder;
            return holder;
        }

        public static void RegisterModifier(Type type, ModPlugin modPlugin)
        {
            BaseModifier baseModifier = (BaseModifier)Activator.CreateInstance(type);
            baseModifier.ModifierId = LastModifierId;
            Modifiers.Add(LastModifierId, baseModifier);
            LastModifierId++;

            baseModifier.CountData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            baseModifier.CountData.Type = OptionTypes.Float;
            baseModifier.CountData.Title = FungleTranslation.QuantityPerGame;
            baseModifier.CountData.Increment = 1;
            baseModifier.CountData.ValidRange = new FloatRange(0, baseModifier.MaxCount);
            baseModifier.CountData.FormatString = null;
            baseModifier.CountData.ZeroIsInfinity = false;
            baseModifier.CountData.SuffixType = NumberSuffixes.None;
            baseModifier.CountData.OptionName = FloatOptionNames.Invalid;

            baseModifier.ChanceData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            baseModifier.ChanceData.Type = OptionTypes.Float;
            baseModifier.ChanceData.Title = FungleTranslation.ChancePerGame;
            baseModifier.ChanceData.Increment = 1;
            baseModifier.ChanceData.ValidRange = new FloatRange(0, 100);
            baseModifier.ChanceData.FormatString = null;
            baseModifier.ChanceData.ZeroIsInfinity = false;
            baseModifier.ChanceData.SuffixType = NumberSuffixes.None;
            baseModifier.ChanceData.OptionName = FloatOptionNames.Invalid;

            baseModifier.ModifierOptions = new ModifierOptionCollection(baseModifier);
            baseModifier.ModifierOptions.Initialize(modPlugin, OptionManager.GetAndInitializeModdedOptions(type, modPlugin));

            modPlugin.Modifiers.Add(baseModifier);

            modPlugin.BasePlugin.Log.LogInfo("Registered Modifier " + type.Name + " Id: " + baseModifier.ModifierId.ToString());
        }
    }
}
