using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Event.Types;
using FungleAPI.Networking;
using FungleAPI.Networking.RPCs;
using FungleAPI.Role;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Player
{
    public static class PlayerUtils
    {
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.Instance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer);
        }
        public static List<DeadBody> GetClosestsDeadBodies(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            List<DeadBody> bodies = new List<DeadBody>();
            foreach (DeadBody body in Helpers.AllDeadBodies)
            {
                if (body != null && body.myCollider.enabled && !PhysicsHelpers.AnythingBetween(target.GetTruePosition(), body.TruePosition, Constants.ShipAndObjectsMask, false) && (!body.Reported || includeReporteds) && Vector2.Distance(target.GetTruePosition(), body.TruePosition) <= distance)
                {
                    bodies.Add(body);
                }
            }
            return bodies;
        }
        public static DeadBody GetClosestDeadBody(this PlayerControl target, float distance, bool includeReporteds = false)
        {
            DeadBody closest = null;
            float dis = distance;
            foreach (DeadBody body in GetClosestsDeadBodies(target, distance, includeReporteds))
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
        public static DeadBody GetBody(this PlayerControl player)
        {
            return Helpers.GetBodyById(player.PlayerId);
        }
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
        public static Vent GetCurrentVent(this PlayerControl player)
        {
            if (player.AmOwner)
            {
                return Vent.currentVent;
            }
            return player.GetPlayerComponent<PlayerHelper>().CurrentVent;
        }
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
                        killer.SetKillTimer(CustomRoleManager.CurrentKillConfig.Cooldown() / 2f);
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
                        killer.SetKillTimer(CustomRoleManager.CurrentKillConfig.Cooldown());
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
                killer.MyPhysics.StartCoroutine(CoPerformCustomKill(killer.KillAnimations[new System.Random().Next(0, killer.KillAnimations.Count - 1)], killer, target, resultFlags, createDeadBody, teleportMurderer).WrapToIl2Cpp());
                killer.logger.Debug(string.Format("{0} succeeded in murdering {1}", killer.PlayerId, target.PlayerId), null);
            }
        }
        public static System.Collections.IEnumerator CoPerformCustomKill(KillAnimation anim, PlayerControl source, PlayerControl target, MurderResultFlags resultFlags, bool createDeadBody, bool teleportMurderer)
        {
            OnPlayerMurdered onPlayerMurdered = new OnPlayerMurdered() { Killer = source, Target = target, ResultFlags = resultFlags };
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
                DeadBody deadBody = Helpers.CreateCustomBody(target, GameManager.Instance.GetDeadBody(source.Data.Role).SafeCast<ViperDeadBody>() != null ? DeadBodyType.Viper : DeadBodyType.Normal);
                source.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
                target.Data.Role.KillAnimSpecialSetup(deadBody, source, target);
                if (PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Detective && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.Data.Disconnected)
                {
                    (PlayerControl.LocalPlayer.Data.Role as DetectiveRole).KillAnimSpecialSetup(deadBody, source, target);
                }
                onPlayerMurdered.Body = deadBody;
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
            EventManager.CallEvent(onPlayerMurdered);
            yield break;
        }
    }
}
