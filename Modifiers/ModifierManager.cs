using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FungleAPI.Modifiers
{
    public static class ModifierManager
    {
        private static readonly Dictionary<uint, Type> ModifierTypes = new();
        private static readonly Dictionary<Type, uint> ModifierIds = new();
        private static readonly Dictionary<byte, List<BaseModifier>> PlayerModifiers = new();
        private static uint _nextModifierId;

        public static IReadOnlyDictionary<uint, Type> RegisteredModifiers => ModifierTypes;

        public static void RegisterModifier(Type type, ModPlugin plugin)
        {
            if (type.IsAbstract || !typeof(BaseModifier).IsAssignableFrom(type) || ModifierIds.ContainsKey(type)) return;
            var id = ++_nextModifierId;
            ModifierTypes[id] = type;
            ModifierIds[type] = id;
            plugin.Modifiers.Add(type);
        }

        public static bool TryGetModifierId(Type type, out uint id) => ModifierIds.TryGetValue(type, out id);

        public static IReadOnlyList<BaseModifier> GetModifiers(PlayerControl player)
        {
            if (player != null && PlayerModifiers.TryGetValue(player.PlayerId, out var modifiers)) return modifiers;
            return Array.Empty<BaseModifier>();
        }

        public static T GetModifier<T>(PlayerControl player) where T : BaseModifier => GetModifiers(player).OfType<T>().FirstOrDefault();

        public static bool HasModifier<T>(PlayerControl player) where T : BaseModifier => GetModifier<T>(player) != null;

        public static bool HasModifier(PlayerControl player, Type type) => GetModifiers(player).FirstOrDefault(m => m.GetType() == type) != null;

        public static bool AddModifier(PlayerControl player, uint modifierId, float duration = -1f)
        {
            if (player == null || !ModifierTypes.TryGetValue(modifierId, out var type)) return false;
            var modifier = (BaseModifier)Activator.CreateInstance(type);
            if (!PlayerModifiers.TryGetValue(player.PlayerId, out var modifiers))
            {
                modifiers = new List<BaseModifier>();
                PlayerModifiers[player.PlayerId] = modifiers;
            }
            if (modifier.Unique && modifiers.Any(existing => existing.TypeId == modifierId)) return false;
            modifier.Player = player;
            modifier.TypeId = modifierId;
            modifier.RemainingDuration = duration >= 0f ? duration : modifier.Duration;
            modifiers.Add(modifier);
            modifier.OnAdded();
            return true;
        }

        public static bool RemoveModifier(PlayerControl player, uint modifierId)
        {
            if (player == null || !PlayerModifiers.TryGetValue(player.PlayerId, out var modifiers)) return false;
            var removed = modifiers.Where(modifier => modifier.TypeId == modifierId).ToArray();
            foreach (var modifier in removed)
            {
                modifier.OnRemoved();
                modifiers.Remove(modifier);
            }
            if (modifiers.Count == 0) PlayerModifiers.Remove(player.PlayerId);
            return removed.Length > 0;
        }

        public static void ClearModifiers(PlayerControl player)
        {
            if (player == null || !PlayerModifiers.TryGetValue(player.PlayerId, out var modifiers)) return;
            foreach (var modifier in modifiers.ToArray()) modifier.OnRemoved();
            PlayerModifiers.Remove(player.PlayerId);
        }

        public static void RpcAddModifier(PlayerControl player, uint modifierId, float duration = -1f)
        {
            if (!AddModifier(player, modifierId, duration)) return;
            Rpc<ModifierRpc>.Instance.SendAdd(player, modifierId, duration);
        }

        public static void RpcRemoveModifier(PlayerControl player, uint modifierId)
        {
            if (!RemoveModifier(player, modifierId)) return;
            Rpc<ModifierRpc>.Instance.SendRemove(player, modifierId);
        }

        public static void RpcClearModifiers(PlayerControl player)
        {
            ClearModifiers(player);
            Rpc<ModifierRpc>.Instance.SendClear(player);
        }

        public static void Update()
        {
            foreach (var modifiers in PlayerModifiers.Values.ToArray())
            foreach (var modifier in modifiers.ToArray())
            {
                modifier.OnUpdated();
                if (modifier.RemainingDuration < 0f) continue;
                modifier.RemainingDuration -= Time.deltaTime;
                if (modifier.RemainingDuration <= 0f) RemoveModifier(modifier.Player, modifier.TypeId);
            }
        }

        public static void NotifyPlayerDied(PlayerControl player, DeathReason reason)
        {
            foreach (var modifier in GetModifiers(player).ToArray()) modifier.OnPlayerDied(reason);
        }

        public static void NotifyMeetingStarted()
        {
            foreach (var modifiers in PlayerModifiers.Values.ToArray())
            foreach (var modifier in modifiers.ToArray()) modifier.OnMeetingStarted();
        }
    }
}
