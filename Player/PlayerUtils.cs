using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Il2CppSystem.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Player
{
    /// <summary>
    /// A player utility class
    /// </summary>
    public static class PlayerUtils
    {
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
        /// Perform a custom kill
        /// </summary>
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            Rpc<RpcCustomMurder>.Instance.Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer);
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
        /// Get a PlayerComponent
        /// </summary>
        public static T GetPlayerComponent<T>(this PlayerControl player) where T : PlayerComponent
        {
            T comp = player.GetComponent<T>();
            if (comp == null)
            {
                PlayerControlPatch.DoStart(player);
                comp = player.GetComponent<T>();
            }
            return comp;
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
            return player.GetPlayerComponent<PlayerHelper>().CurrentVent;
        }
        /// <summary>
        /// Perform a custom kill
        /// </summary>
        public static void CustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound)
        {
            killer.isKilling = false;
            killer.logger.Debug(string.Format("{0} trying to murder {1}", killer.PlayerId, target.PlayerId), null);
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
                if (killer.AmOwner || flag)
                {
                    target.ShowFailedMurder();
                    if (resetKillTimer)
                    {
                        killer.SetKillTimer(RoleConfigManager.KillConfig.Cooldown() / 2f);
                    }
                }
                else
                {
                    target.RemoveProtection();
                }
                killer.logger.Debug(string.Format("{0} failed to murder {1} due to guardian angel protection", killer.PlayerId, target.PlayerId), null);
                return;
            }
            if (resultFlags.HasFlag(MurderResultFlags.Succeeded) || resultFlags.HasFlag(MurderResultFlags.DecisionByHost))
            {
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.Kill(target.Data, killer.Data);
                if (killer.AmOwner)
                {
                    if (GameManager.Instance.IsHideAndSeek())
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.HideAndSeek_ImpostorKills);
                    }
                    else
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.ImpostorKills);
                    }
                    if (killer.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.Role_Shapeshifter_ShiftedKills);
                    }
                    if (Constants.ShouldPlaySfx() && playKillSound)
                    {
                        SoundManager.Instance.PlaySound(killer.KillSfx, false, 0.8f, null);
                    }
                    if (resetKillTimer)
                    {
                        killer.SetKillTimer(RoleConfigManager.KillConfig.Cooldown());
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
                        DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    }
                    target.cosmetics.SetNameMask(false);
                    target.RpcSetScanner(false);
                }
                DestroyableSingleton<AchievementManager>.Instance.OnMurder(killer.AmOwner, target.AmOwner, killer.CurrentOutfitType == PlayerOutfitType.Shapeshifted, killer.shapeshiftTargetPlayerId, (int)target.PlayerId);
                killer.MyPhysics.StartCoroutine(CoPerformCustomKill(killer.KillAnimations.Random(), killer, target, resultFlags, createDeadBody, teleportMurderer).WrapToIl2Cpp());
                killer.logger.Debug(string.Format("{0} succeeded in murdering {1}", killer.PlayerId, target.PlayerId), null);
            }
        }
        public static System.Collections.IEnumerator CoPerformCustomKill(KillAnimation anim, PlayerControl source, PlayerControl target, MurderResultFlags resultFlags, bool createDeadBody, bool teleportMurderer)
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
            if (createDeadBody)
            {
                DeadBody deadBody = BodyUtils.CreateDeadBody(target, source.Data.Role.GetCreatedDeadBody());
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
            yield return source.MyPhysics.Animations.CoPlayCustomAnimation(anim.BlurAnim);
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
            yield break;
        }
    }
}
