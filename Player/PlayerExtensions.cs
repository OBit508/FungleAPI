using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.GameOver;
using FungleAPI.Networking;
using FungleAPI.Player.Networking;
using FungleAPI.Player.Networking.Data;
using FungleAPI.Player.Patches;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Player
{
    /// <summary>
    /// Extensions for the Player
    /// </summary>
    public static class PlayerExtensions
    {
        // RPCs

        /// <summary>
        /// Request to the host to end the game
        /// </summary>
        public static void RpcRequestGameOver(this PlayerControl source, CustomGameOver customGameOver)
        {
            if (AmongUsClient.Instance.AmLocalHost || AmongUsClient.Instance.AmModdedHost)
            {
                GameManager.Instance?.RpcEndGame(customGameOver);
                return;
            }
            Rpc<RpcRequestGameOver>.Instance.Send(customGameOver, source);
        }
        /// <summary>
        /// Request to the host to end the game
        /// </summary>
        public static void RpcRequestGameOver<T>(this PlayerControl source) where T : CustomGameOver
        {
            source.RpcRequestGameOver(GameOverManager.GetGameOverInstance<T>());
        }
        /// <summary>
        /// Request to the host to end the game
        /// </summary>
        public static void RpcRequestGameOver(this PlayerControl source, Type type)
        {
            source.RpcRequestGameOver(GameOverManager.GetGameOverInstance(type));
        }

        /// <summary>
        /// Makes the player move to the given vent
        /// </summary>
        public static void RpcMoveToVent(this PlayerControl source, VentHelper ventHelper)
        {
            if (EventManager.CallEvent(new BeforeMoveVent(ventHelper.vent)).Cancelled)
            {
                return;
            }
            Rpc<RpcMoveToVent>.Instance.Send(ventHelper, source);
        }
        /// <summary>
        /// Perform a custom murder
        /// </summary>
        public static void RpcCustomMurderPlayer(this PlayerControl source, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            if (EventManager.CallEvent(new BeforeMurderEvent(target, resultFlags)).Cancelled)
            {
                return;
            }
            Rpc<RpcCustomMurder>.Instance.Send(new MurderData(target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), source);
        }
        /// <summary>
        /// Perform a custom murder
        /// </summary>
        public static void CmdCheckCustomMurder(this PlayerControl source, PlayerControl target, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            source.isKilling = true;
            if (AmongUsClient.Instance.AmLocalHost || AmongUsClient.Instance.AmModdedHost)
            {
                source.CheckCustomMurder(target, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound);
                return;
            }
            Rpc<CmdCustomMurder>.Instance.Send(new MurderData(target, MurderResultFlags.NULL, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), source);
        }





        // Utils

        /// <summary>
        /// Get a player by the Id
        /// </summary>
        public static PlayerControl GetPlayerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }
            return null;
        }
        /// <summary>
        /// Get all nearby dead bodies
        /// </summary>
        public static List<DeadBody> GetAllNearbyDeadBodies(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            List<DeadBody> bodies = new List<DeadBody>();
            foreach (DeadBody body in BodyUtils.AllDeadBodies)
            {
                if (body != null && body.myCollider.enabled && !PhysicsHelpers.AnythingBetween(target.GetTruePosition(), body.TruePosition, Constants.ShipAndObjectsMask, false) && (!body.Reported || includeReporteds) && Vector2.Distance(target.GetTruePosition(), body.TruePosition) <= distance)
                {
                    bodies.Add(body);
                }
            }
            return bodies;
        }
        /// <summary>
        /// Get the cosest dead body
        /// </summary>
        public static DeadBody GetClosestDeadBody(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            DeadBody closest = null;
            float dis = distance;
            foreach (DeadBody body in GetAllNearbyDeadBodies(target, distance, includeReporteds))
            {
                float d = Vector2.Distance(target.GetTruePosition(), body.TruePosition);
                if (dis > d)
                {
                    dis = d;
                    closest = body;
                }
            }
            return closest;
        }
        /// <summary>
        /// Get the dead body
        /// </summary>
        public static DeadBody GetBody(this PlayerControl player)
        {
            return BodyUtils.GetBodyById(player.PlayerId);
        }
        /// <summary>
        /// Get the vote area on meeting
        /// </summary>
        public static PlayerVoteArea GetVoteArea(this PlayerControl player)
        {
            if (MeetingHud.Instance)
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    if (playerVoteArea.TargetPlayerId == player.PlayerId)
                    {
                        return playerVoteArea;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Get the chat bubble on the chat tab
        /// </summary>
        public static List<ChatBubble> GetChatBubble(this PlayerControl player)
        {
            List<ChatBubble> list = new List<ChatBubble>();
            foreach (ChatBubble chatBubble in HudManager.Instance.Chat.chatBubblePool.activeChildren)
            {
                if (chatBubble.playerInfo.Object.PlayerId == player.PlayerId)
                {
                    list.Add(chatBubble);
                }
            }
            return list;
        }
        /// <summary>
        /// Get the current vent
        /// </summary>
        public static Vent GetCurrentVent(this PlayerControl player)
        {
            if (player.AmOwner)
            {
                return Vent.currentVent;
            }
            return player.GetComponent<PlayerHelper>().CurrentVent;
        }





        // Helpers

        public static void CheckCustomMurder(this PlayerControl source, PlayerControl target, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            source.logger.Debug(string.Format("Checking if {0} murdered {1}", source.PlayerId, (target == null) ? "null player" : target.PlayerId.ToString()), null);
            source.isKilling = false;
            if (AmongUsClient.Instance.IsGameOver || !AmongUsClient.Instance.AmHost)
            {
                return;
            }
            if (!target || source.Data.IsDead || source.Data.Disconnected)
            {
                int num = target ? ((int)target.PlayerId) : -1;
                source.logger.Warning(string.Format("Bad kill from {0} to {1}", source.PlayerId, num), null);
                source.RpcMurderPlayer(target, false);
                return;
            }
            NetworkedPlayerInfo data = target.Data;
            if (data == null || data.IsDead || target.inVent || target.MyPhysics.Animations.IsPlayingEnterVentAnimation() || target.MyPhysics.Animations.IsPlayingAnyLadderAnimation() || target.inMovingPlat)
            {
                source.logger.Warning("Invalid target data for kill", null);
                source.RpcMurderPlayer(target, false);
                return;
            }
            if (MeetingHud.Instance)
            {
                source.logger.Warning("Tried to kill while a meeting was starting", null);
                source.RpcMurderPlayer(target, false);
                return;
            }
            source.isKilling = true;
            source.RpcCustomMurderPlayer(target, MurderResultFlags.DecisionByHost | MurderResultFlags.Succeeded, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound);
        }
        public static void CustomMurderPlayer(this PlayerControl source, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            source.isKilling = false;
            source.logger.Debug(string.Format("{0} trying to murder {1}", source.PlayerId, target.PlayerId), null);
            NetworkedPlayerInfo data = target.Data;
            if (resultFlags.HasFlag(MurderResultFlags.FailedError))
            {
                return;
            }
            if (resultFlags.HasFlag(MurderResultFlags.FailedProtected) || (resultFlags.HasFlag(MurderResultFlags.DecisionByHost) && target.protectedByGuardianId > -1))
            {
                target.protectedByGuardianThisRound = true;
                bool flag = PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel;
                if (flag && (int)PlayerControl.LocalPlayer.Data.PlayerId == target.protectedByGuardianId)
                {
                    DataManager.Player.Stats.IncrementStat(StatID.Role_GuardianAngel_CrewmatesProtected);
                    DestroyableSingleton<AchievementManager>.Instance.OnProtectACrewmate();
                }
                if (source.AmOwner || flag)
                {
                    target.ShowFailedMurder();
                    if (resetKillTimer)
                    {
                        source.SetKillTimer(RoleConfigManager.KillConfig.Cooldown() / 2f);
                    }
                }
                else
                {
                    target.RemoveProtection();
                }
                source.logger.Debug(string.Format("{0} failed to murder {1} due to guardian angel protection", source.PlayerId, target.PlayerId), null);
                return;
            }
            if (resultFlags.HasFlag(MurderResultFlags.Succeeded) || resultFlags.HasFlag(MurderResultFlags.DecisionByHost))
            {
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.Kill(target.Data, source.Data);
                if (source.AmOwner)
                {
                    if (GameManager.Instance.IsHideAndSeek())
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.HideAndSeek_ImpostorKills);
                    }
                    else
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.ImpostorKills);
                    }
                    if (source.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.Role_Shapeshifter_ShiftedKills);
                    }
                    if (Constants.ShouldPlaySfx() && playKillSound)
                    {
                        SoundManager.Instance.PlaySound(source.KillSfx, false, 0.8f, null);
                    }
                    if (resetKillTimer)
                    {
                        source.SetKillTimer(RoleConfigManager.KillConfig.Cooldown());
                    }
                }
                DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                if (target.AmOwner)
                {
                    DataManager.Player.Stats.IncrementStat(StatID.TimesMurdered);
                    if (Minigame.Instance)
                    {
                        try
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }
                        catch
                        {
                        }
                    }
                    if (showKillAnim)
                    {
                        DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(source.Data, data);
                    }
                    target.cosmetics.SetNameMask(false);
                    target.RpcSetScanner(false);
                }
                DestroyableSingleton<AchievementManager>.Instance.OnMurder(source.AmOwner, target.AmOwner, source.CurrentOutfitType == PlayerOutfitType.Shapeshifted, source.shapeshiftTargetPlayerId, (int)target.PlayerId);
                source.MyPhysics.StartCoroutine(source.KillAnimations.Random().CoPerformCustomKill(source, target, resultFlags, createDeadBody, teleportMurderer).WrapToIl2Cpp());
                source.logger.Debug(string.Format("{0} succeeded in murdering {1}", source.PlayerId, target.PlayerId), null);
            }
        }
        public static System.Collections.IEnumerator CoPerformCustomKill(this KillAnimation killAnimation, PlayerControl source, PlayerControl target, MurderResultFlags resultFlags, bool createDeadBody, bool teleportMurderer)
        {
            FollowerCamera cam = Camera.main.GetComponent<FollowerCamera>();
            bool isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
            PlayerPhysics sourcePhys = source.MyPhysics;
            KillAnimation.SetMovement(source, false);
            KillAnimation.SetMovement(target, false);
            if (isParticipant)
            {
                PlayerControl.LocalPlayer.isKilling = true;
                source.isKilling = true;
            }
            DeadBody deadBody = null;
            if (createDeadBody)
            {
                deadBody = BodyUtils.CreateDeadBody(target, source.Data.Role.GetCreatedDeadBody());
                source.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
                target.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
                if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Detective && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected)
                {
                    (PlayerControl.LocalPlayer.Data.Role as DetectiveRole).KillAnimSpecialSetup(deadBody, source, target);
                }
            }
            if (isParticipant)
            {
                cam.Locked = true;
                ConsoleJoystick.SetMode_Task();
                if (PlayerControl.LocalPlayer.AmOwner)
                {
                    PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
                }
            }
            target.Die(DeathReason.Kill, true);
            yield return source.MyPhysics.Animations.CoPlayCustomAnimation(killAnimation.BlurAnim);
            if (teleportMurderer)
            {
                source.NetTransform.SnapTo(target.transform.position);
            }
            sourcePhys.Animations.PlayIdleAnimation();
            KillAnimation.SetMovement(source, true);
            KillAnimation.SetMovement(target, true);
            if (isParticipant)
            {
                cam.Locked = false;
                PlayerControl.LocalPlayer.isKilling = false;
                source.isKilling = false;
            }
            EventManager.CallEvent(new AfterMurderEvent(source, target, deadBody, resultFlags));
            yield break;
        }
    }
}
