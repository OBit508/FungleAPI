using AmongUs.GameOptions;
using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Roles;
using FungleAPI.Translation;
using Il2CppInterop.Runtime;
using Il2CppSystem.CodeDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace FungleAPI.Rpc
{
    public static class CustomRpcManager
    {
        internal static List<CustomRpc> AllRpc = new List<CustomRpc>();
        public static CustomRpc CreateRpc(ModPlugin plugin, RpcType rpcType, Action onReceive)
        {
            CustomRpc rpc = new CustomRpc(plugin, rpcType);
            rpc.onReceive = onReceive;
            if (onReceive == null)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to create rpc, onReceive is null.");
            }
            AllRpc.Add(rpc);
            return rpc;
        }
        internal static void LoadModRpcs()
        {
            rpcSetSpeed = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                PlayerControl player = rpcSetSpeed.ReadPlayer();
                float speed = rpcSetSpeed.ReadFloat();
                float ghostSpeed = rpcSetSpeed.ReadFloat();
                player.MyPhysics.Speed = speed;
                player.MyPhysics.GhostSpeed = ghostSpeed;
            });
            rpcSetScale = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                    PlayerControl player = rpcSetScale.ReadPlayer();
                    float x = rpcSetScale.ReadFloat();
                    float y = rpcSetScale.ReadFloat();
                    player.transform.localScale = new Vector3(x, y, 0.7f);
            });
            rpcRevive = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                PlayerControl player = rpcRevive.ReadPlayer();
                bool destroyBody = rpcRevive.ReadBool();
                player.Revive();
                if (destroyBody && player.GetBody() != null)
                {
                    player.GetBody().Destroy();
                }
            });
            rpcCarryBody = CreateRpc(FungleAPIPlugin.Plugin, RpcType.GameManager, delegate
            {
                CustomDeadBody body = rpcCarryBody.ReadBody();
                PlayerControl player = rpcCarryBody.ReadPlayer();
                body.Carring = player;
            });
            rpcStopCarryBody = CreateRpc(FungleAPIPlugin.Plugin, RpcType.GameManager, delegate
            {
                CustomDeadBody body = rpcStopCarryBody.ReadBody();
                float x = rpcStopCarryBody.ReadFloat();
                float y = rpcStopCarryBody.ReadFloat();
                body.Carring = null;
                body.transform.position = new Vector3(x, y, y / 1000f);
            });
            rpcCustomEndGame = CreateRpc(FungleAPIPlugin.Plugin, RpcType.GameManager, delegate
            {
                int count = rpcCustomEndGame.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    EndGamePatch.Winners.Add(new CachedPlayerData(rpcCustomEndGame.ReadNetObject()));
                }
            });
            rpcSendNotification = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                string text = rpcSendNotification.ReadString();
                bool playSound = rpcSendNotification.ReadBool();
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
            });
            rpcSetNewRoleValue = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                ICustomRole role = CustomRoleManager.GetRole((RoleTypes)rpcSetNewRoleValue.ReadInt());
                string configName = rpcSetNewRoleValue.ReadString();
                string notificationText = rpcSetNewRoleValue.ReadString();
                foreach (Config config in role.CachedConfig.Configs)
                {
                    if (config.ConfigName == configName)
                    {
                        if (config is NumConfig n)
                        {
                            n.ConfigEntry.Value = rpcSetNewRoleValue.ReadFloat();
                        }
                        else if (config is BoolConfig b)
                        {
                            b.ConfigEntry.Value = rpcSetNewRoleValue.ReadBool();
                        }
                        else if (config is EnumConfig e)
                        {
                            e.ConfigEntry.Value = rpcSetNewRoleValue.ReadString();
                        }
                    }
                }
                if (!notificationText.IsNullOrWhiteSpace())
                {
                    HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, notificationText, true);
                }
            });
            rpcSyncRoleSettings = CreateRpc(FungleAPIPlugin.Plugin, RpcType.Player, delegate
            {
                int count = rpcSyncRoleSettings.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    ICustomRole role = CustomRoleManager.GetRole((RoleTypes)rpcSyncRoleSettings.ReadInt());
                    foreach (Config config in role.CachedConfig.Configs)
                    {
                        if (config is NumConfig n)
                        {
                            n.ConfigEntry.Value = rpcSyncRoleSettings.ReadFloat();
                        }
                        else if (config is BoolConfig b)
                        {
                            b.ConfigEntry.Value = rpcSyncRoleSettings.ReadBool();
                        }
                        else if (config is EnumConfig e)
                        {
                            e.ConfigEntry.Value = rpcSyncRoleSettings.ReadString();
                        }
                    }
                }
            });
        }
        public static void RpcSyncAllRoleSettings()
        {
            List<ICustomRole> roles = new List<ICustomRole>();
            foreach (RoleBehaviour role in CustomRoleManager.AllRoles)
            {
                if (role as ICustomRole != null)
                {
                    roles.Add(role as ICustomRole);
                }
            }
            rpcSyncRoleSettings.Write(roles.Count);
            foreach (ICustomRole role in roles)
            {
                rpcSyncRoleSettings.Write((int)role.RoleType);
                foreach (Config config in role.CachedConfig.Configs)
                {
                    if (config is NumConfig n)
                    {
                        rpcSyncRoleSettings.Write(n.ConfigEntry.Value);
                    }
                    else if (config is BoolConfig b)
                    {
                        rpcSyncRoleSettings.Write(b.ConfigEntry.Value);
                    }
                    else if (config is EnumConfig e)
                    {
                        rpcSyncRoleSettings.Write(e.ConfigEntry.Value);
                    }
                }
            }
            rpcSyncRoleSettings.SendRpc();
        }
        public static void RpcSetNewRoleValue(this ICustomRole role, Config config, object value, string notificationText = "")
        {
            rpcSetNewRoleValue.Write((int)role.RoleType);
            rpcSetNewRoleValue.Write(config.ConfigName);
            rpcSetNewRoleValue.Write(notificationText);
            if (config is NumConfig n)
            {
                rpcSetNewRoleValue.Write((int)value);
                n.ConfigEntry.Value = (int)value;
            }
            else if (config is BoolConfig b)
            {
                rpcSetNewRoleValue.Write((bool)value);
                b.ConfigEntry.Value = (bool)value;
            }
            else if (config is EnumConfig e)
            {
                rpcSetNewRoleValue.Write((string)value);
                e.ConfigEntry.Value = (string)value;
            }
            if (!notificationText.IsNullOrWhiteSpace())
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, notificationText, true);
            }
            rpcSetNewRoleValue.SendRpc();
        }
        public static void RpcSendNotification(this HudManager hud, string text, bool playSound = true)
        {
            rpcSendNotification.Write(text);
            rpcSendNotification.Write(playSound);
            rpcSendNotification.SendRpc();
            hud.Notifier.SettingsChangeMessageLogic(StringNames.None, text, playSound);
        }
        
        public static void RpcCarryBody(this CustomDeadBody body, PlayerControl player)
        {
            rpcCarryBody.Write(body);
            rpcCarryBody.Write(player);
            body.Carring = player;
            rpcCarryBody.SendRpc();
        }
        public static void RpcStopCarryBody(this CustomDeadBody body)
        {
            rpcStopCarryBody.Write(body);
            rpcStopCarryBody.Write(body.transform.position.x);
            rpcStopCarryBody.Write(body.transform.position.y);
            body.Carring = null;
            rpcStopCarryBody.SendRpc();
        }
        public static void RpcSetSpeed(this PlayerControl player, float speed, float ghostSpeed)
        {
            rpcSetSpeed.Write(player);
            rpcSetSpeed.Write(speed);
            rpcSetSpeed.Write(ghostSpeed);
            player.MyPhysics.Speed = speed;
            player.MyPhysics.GhostSpeed = ghostSpeed;
            rpcSetSpeed.SendRpc();
        }
        public static void RpcSetScale(this PlayerControl player, Vector2 scale)
        {
            rpcSetScale.Write(player);
            rpcSetScale.Write(scale.x);
            rpcSetScale.Write(scale.y);
            player.transform.localScale = new Vector3(scale.x, scale.y, 0.7f);
            rpcSetScale.SendRpc();
        }
        public static void RpcRevive(this PlayerControl player, bool destroyBody)
        {
            rpcRevive.Write(player);
            rpcRevive.Write(destroyBody);
            player.Revive();
            if (destroyBody && player.GetBody() != null)
            {
                player.GetBody().Destroy();
            }
            rpcRevive.SendRpc();
        }
        internal static CustomRpc rpcSetSpeed;
        internal static CustomRpc rpcSetScale;
        internal static CustomRpc rpcRevive;
        internal static CustomRpc rpcCarryBody;
        internal static CustomRpc rpcStopCarryBody;
        internal static CustomRpc rpcSetNewRoleValue;
        internal static CustomRpc rpcSyncRoleSettings;
        internal static CustomRpc rpcCustomEndGame;
        internal static CustomRpc rpcSendNotification;
    }
}
