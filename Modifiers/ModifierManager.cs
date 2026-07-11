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
        private static readonly Dictionary<uint, Type> ModifierTypes = new Dictionary<uint, Type>();
        private static readonly Dictionary<Type, uint> ModifierIds = new Dictionary<Type, uint>();
        private static readonly Dictionary<Type, CachedModifier> CachedModifiers = new Dictionary<Type, CachedModifier>();
        private static readonly Dictionary<byte, List<BaseModifier>> PlayerModifiers = new Dictionary<byte, List<BaseModifier>>();
        private static uint _nextModifierId;

        public static IReadOnlyDictionary<uint, Type> RegisteredModifiers => ModifierTypes;

        public static void RegisterModifier(Type type, ModPlugin plugin)
        {
            if (type.IsAbstract || !typeof(BaseModifier).IsAssignableFrom(type) || ModifierIds.ContainsKey(type))
            {
                return;
            }

            BaseModifier baseModifier = (BaseModifier)Activator.CreateInstance(type);

            uint id = ++_nextModifierId;
            ModifierTypes[id] = type;
            ModifierIds[type] = id;
            CachedModifiers[type] = new CachedModifier() { ModifierName = baseModifier.ModifierName, ModifierColor = baseModifier.ModifierColor, ModifierId = id };
            plugin.Modifiers.Add(type);
        }

        public static bool TryGetModifierId(Type type, out uint id)
        {
            return ModifierIds.TryGetValue(type, out id);
        }

        public static bool TryGetCachedModifier(Type type, out CachedModifier cachedModifier)
        {
            return CachedModifiers.TryGetValue(type, out cachedModifier);
        }

        public static bool HasModifier(PlayerControl player, uint modifierId)
        {
            return player != null && PlayerModifiers.TryGetValue(player.PlayerId, out List<BaseModifier> modifiers) && modifiers.Any(modifier => modifier.TypeId == modifierId);
        }

        public static IReadOnlyList<BaseModifier> GetModifiers(PlayerControl player)
        {
            if (player != null && PlayerModifiers.TryGetValue(player.PlayerId, out List<BaseModifier> modifiers))
            {
                return modifiers;
            }
            return Array.Empty<BaseModifier>();
        }

        public static bool AddModifier(PlayerControl player, uint modifierId, float duration = -1f)
        {
            if (player == null || !ModifierTypes.TryGetValue(modifierId, out Type type))
            {
                return false;
            }

            BaseModifier modifier = (BaseModifier)Activator.CreateInstance(type);
            if (!PlayerModifiers.TryGetValue(player.PlayerId, out List<BaseModifier> modifiers))
            {
                modifiers = new List<BaseModifier>();
                PlayerModifiers[player.PlayerId] = modifiers;
            }

            if (modifier.Unique && modifiers.Any(existing => existing.TypeId == modifierId))
            {
                return false;
            }

            modifier.Player = player;
            modifier.TypeId = modifierId;
            modifier.RemainingDuration = duration >= 0f ? duration : modifier.Duration;
            modifiers.Add(modifier);
            modifier.OnAdded();
            return true;
        }

        public static bool RemoveModifier(PlayerControl player, uint modifierId)
        {
            if (player == null || !PlayerModifiers.TryGetValue(player.PlayerId, out List<BaseModifier> modifiers))
            {
                return false;
            }

            List<BaseModifier> removed = modifiers.Where(modifier => modifier.TypeId == modifierId).ToList();
            foreach (BaseModifier modifier in removed)
            {
                modifier.OnRemoved();
                modifiers.Remove(modifier);
            }
            if (modifiers.Count == 0)
            {
                PlayerModifiers.Remove(player.PlayerId);
            }
            return removed.Count > 0;
        }

        public static void RpcAddModifier(PlayerControl player, uint modifierId, float duration = -1f)
        {
            if (AddModifier(player, modifierId, duration))
            {
                Rpc<ModifierRpc>.Instance.SendAdd(player.PlayerId, modifierId, duration);
            }
        }

        public static void RpcRemoveModifier(PlayerControl player, uint modifierId)
        {
            if (RemoveModifier(player, modifierId))
            {
                Rpc<ModifierRpc>.Instance.SendRemove(player.PlayerId, modifierId);
            }
        }

        internal static void Update()
        {
            foreach (List<BaseModifier> modifiers in PlayerModifiers.Values.ToArray())
            {
                foreach (BaseModifier modifier in modifiers.ToArray())
                {
                    modifier.OnUpdated();
                    if (modifier.RemainingDuration < 0f)
                    {
                        continue;
                    }
                    modifier.RemainingDuration -= Time.deltaTime;
                    if (modifier.RemainingDuration <= 0f)
                    {
                        RemoveModifier(modifier.Player, modifier.TypeId);
                    }
                }
            }
        }

        internal static void NotifyPlayerDied(PlayerControl player, DeathReason reason)
        {
            foreach (BaseModifier modifier in GetModifiers(player).ToArray())
            {
                modifier.OnPlayerDied(reason);
            }
        }

        internal static void NotifyMeetingStarted()
        {
            foreach (List<BaseModifier> modifiers in PlayerModifiers.Values)
            {
                foreach (BaseModifier modifier in modifiers)
                {
                    modifier.OnMeetingStarted();
                }
            }
        }
    }
}
