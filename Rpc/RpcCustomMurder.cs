using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role.RoleEvent;
using FungleAPI.Roles;
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

namespace FungleAPI.Rpc
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
        }
        public override void Read(MessageReader reader)
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
        public override void Handle((PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound) value)
        {
            CustomMurder(value.killer, value.target, value.resultFlags, value.resetKillTimer, value.createDeadBody, value.teleportMurderer, value.showKillAnim, value.playKillSound);
        }
        public static void CustomMurder(PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer, bool createDeadBody, bool teleportMurderer, bool showKillAnim, bool playKillSound)
        {
            if (killer.Data.Role.InvokeMurderPlayerEvent(target, resultFlags, false))
            {
                if (resultFlags == MurderResultFlags.FailedError || resultFlags == MurderResultFlags.NULL)
                {
                    return;
                }
                bool hostCanKill = killer.Data.Role.CanKill() && target.protectedByGuardianId == -1;
                bool flag = target.protectedByGuardianId == PlayerControl.LocalPlayer.PlayerId;
                bool flag2 = killer == PlayerControl.LocalPlayer;
                bool flag3 = target == PlayerControl.LocalPlayer;
                if (resultFlags == MurderResultFlags.FailedProtected || resultFlags == MurderResultFlags.DecisionByHost && !hostCanKill)
                {
                    target.protectedByGuardianThisRound = true;
                    if (flag)
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.Role_GuardianAngel_CrewmatesProtected);
                        AchievementManager.Instance.OnProtectACrewmate();
                    }
                    if (flag2 || flag)
                    {
                        target.ShowFailedMurder();
                        if (flag2 && resetKillTimer)
                        {
                            killer.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
                        }
                    }
                    target.RemoveProtection();
                }
                else if (resultFlags == MurderResultFlags.Succeeded || resultFlags == MurderResultFlags.DecisionByHost && hostCanKill)
                {
                    killer.isKilling = true;
                    FollowerCamera cam = Camera.main?.GetComponent<FollowerCamera>();
                    if (teleportMurderer)
                    {
                        killer.MyPhysics.Animations.PlayIdleAnimation();
                        killer.NetTransform.SnapTo(target.GetTruePosition());
                        killer.NetTransform.SnapTo(target.transform.position);
                        KillAnimation.SetMovement(killer, true);
                    }
                    KillAnimation.SetMovement(target, true);
                    if (createDeadBody)
                    {
                        CustomDeadBody.CreateCustomBody(target);
                    }
                    if (playKillSound && Constants.ShouldPlaySfx() && flag2)
                    {
                        SoundManager.Instance.PlaySound(killer.KillSfx, false, 0.8f);
                    }
                    if (flag2 || flag3)
                    {
                        ConsoleJoystick.SetMode_Task();
                        cam.Locked = true;
                        if (flag2)
                        {
                            if (PlayerControl.LocalPlayer.Data.RoleType == RoleTypes.Shapeshifter)
                            {
                                DataManager.Player.Stats.IncrementStat(StatID.Role_Shapeshifter_ShiftedKills);
                            }
                            DataManager.Player.Stats.IncrementStat(GameManager.Instance.IsHideAndSeek() ? StatID.HideAndSeek_ImpostorKills : StatID.ImpostorKills);
                        }
                        else if (flag3)
                        {
                            if (showKillAnim)
                            {
                                HudManager.Instance.KillOverlay.ShowKillAnimation(killer.Data, target.Data);
                            }
                            DataManager.Player.Stats.IncrementStat(StatID.TimesMurdered);
                            PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
                        }
                    }
                    DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
                    target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    target.cosmetics.SetNameMask(false);
                    target.RpcSetScanner(false);
                    DestroyableSingleton<AchievementManager>.Instance.OnMurder(flag2, target.AmOwner, killer.CurrentOutfitType == PlayerOutfitType.Shapeshifted, killer.shapeshiftTargetPlayerId, target.PlayerId);
                    target.Die(DeathReason.Kill, true);
                    if (flag2 || flag3)
                    {
                        cam.Locked = false;
                    }
                    killer.isKilling = false;
                }
                killer.Data.Role.InvokeMurderPlayerEvent(target, resultFlags, true);
            }
        }
    }
}
