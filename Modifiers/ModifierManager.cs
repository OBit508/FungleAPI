using AmongUs.GameOptions;
using FungleAPI.Components;
using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using FungleAPI.Modifiers.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Utilities;
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

            baseModifier.ModifierOptions = new ModifierOptionCollection(baseModifier);
            baseModifier.ModifierOptions.Initialize(modPlugin, OptionManager.GetAndInitializeModdedOptions(type, modPlugin));

            modPlugin.Modifiers.Add(baseModifier);

            modPlugin.BasePlugin.Log.LogInfo("Registered Modifier " + type.Name + " Id: " + baseModifier.ModifierId.ToString());
        }
    }
}
