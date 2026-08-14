using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.GameOptions.Collections;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Il2CppSystem.Linq.Expressions.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Modifiers
{
    [FungleIgnore]
    public abstract class BaseModifier
    {
        internal FloatGameSetting CountData;
        internal FloatGameSetting ChanceData;

        public PlayerControl Player;
        public uint ModifierId { get; internal set; }
        public ModifierOptionCollection ModifierOptions { get; internal set; }

        public abstract StringNames ModifierName { get; }
        public abstract StringNames ModifierBlur { get; }
        public virtual Color ModifierColor => Color.gray;

        public virtual bool HideInFreeplay { get; }
        public virtual bool HideInLobby { get; }

        public virtual byte DefaultCount { get; }
        public virtual byte DefaultChance { get; }
        public virtual byte MaxCount => 15;

        public virtual ModdedTeam SpecificTeam { get; }

        public virtual bool ForceCanKill { get; }
        public virtual bool ForceCanSabotage { get; }
        public virtual bool ForceCanVent { get; }

        public virtual void OnDeath(DeathReason reason) { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Initialize(PlayerControl owner)
        {
            Player = owner;
        }
        public virtual void Deinitialize() { }
        public virtual void AppendHint(Il2CppSystem.Text.StringBuilder stringBuilder) 
        {
            stringBuilder.AppendLine($"{ModifierColor.ToTextColor()}<b>{ModifierName.GetString()}</b></color>");
            stringBuilder.AppendLine($"<size=70%>{ModifierBlur.GetString()}</size>");
            stringBuilder.AppendLine();
        }

        public virtual int GetCount() => AmongUsClient.Instance.AmHost ? ModifierOptions.LocalModifierCount.Value : ModifierOptions.NonHostModifierCount;
        public virtual int GetChance() => AmongUsClient.Instance.AmHost ? ModifierOptions.LocalModifierChance.Value : ModifierOptions.NonHostModifierChance;
    }
}
