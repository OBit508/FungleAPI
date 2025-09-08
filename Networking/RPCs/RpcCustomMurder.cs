using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using FungleAPI.Components;
using FungleAPI.Role;
using FungleAPI.Networking;
using FungleAPI.Utilities;
using Hazel;
using MS.Internal.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;
using static UnityEngine.GraphicsBuffer;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace FungleAPI.Networking.RPCs
{
    public class RpcCustomMurder : CustomRpc<(PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound)>
    {
        public override void Write(MessageWriter writer, (PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound) value)
        {
            writer.WritePlayer(value.killer);
            writer.WritePlayer(value.target);
            writer.Write((int)value.resultFlags);
            writer.Write(value.resetKillTimer);
            writer.Write(value.createDeadBody);
            writer.Write(value.teleportMurderer);
            writer.Write(value.showKillAnim);
            writer.Write(value.playKillSound);
            CustomMurder(value.killer, value.target, value.resultFlags, value.resetKillTimer, value.createDeadBody, value.teleportMurderer, value.showKillAnim, value.playKillSound);
        }
        public override void Handle(MessageReader reader)
        {
            PlayerControl killer = reader.ReadPlayer();
            PlayerControl target = reader.ReadPlayer();
            MurderResultFlags resultFlags = (MurderResultFlags)reader.ReadInt32();
            bool resetKillTimer = reader.ReadBoolean();
            bool createDeadBody = reader.ReadBoolean();
            bool teleportMurderer = reader.ReadBoolean();
            bool showKillAnim = reader.ReadBoolean();
            bool playKillSound = reader.ReadBoolean();
            CustomMurder(killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound);
        }
        public static void CustomMurder(PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound)
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
                        killer.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
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
                        killer.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
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
                killer.MyPhysics.StartCoroutine(CoPerformCustomKill(killer.KillAnimations[new System.Random().Next(0, killer.KillAnimations.Count - 1)], killer, target, createDeadBody, teleportMurderer).WrapToIl2Cpp());
                killer.logger.Debug(string.Format("{0} succeeded in murdering {1}", killer.PlayerId, target.PlayerId), null);
            }
        }
        public static System.Collections.IEnumerator CoPerformCustomKill(KillAnimation anim, PlayerControl source, PlayerControl target, bool createDeadBody, bool teleportMurderer)
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
                Helpers.CreateCustomBody(target);
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
