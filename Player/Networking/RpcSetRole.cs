using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using FungleAPI.Player.Networking.Data;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Player.Networking
{
    internal class RpcSetRole : AdvancedRpc<SetRoleData, PlayerControl>
    {
        public override void Write(PlayerControl innerNetObject, MessageWriter messageWriter, SetRoleData data)
        {
            messageWriter.WritePlayer(data.Source);
            messageWriter.Write((ushort)data.RoleType);
            messageWriter.Write(data.ShowIntro);
            innerNetObject.StartCoroutine(CoSetRole(innerNetObject, data.RoleType, data.ShowIntro).WrapToIl2Cpp());
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            PlayerControl source = messageReader.ReadPlayer();
            RoleTypes roleTypes = (RoleTypes)messageReader.ReadUInt16();
            bool showIntro = messageReader.ReadBoolean();

            source.StartCoroutine(CoSetRole(source, roleTypes, showIntro).WrapToIl2Cpp());
        }
        public System.Collections.IEnumerator CoSetRole(PlayerControl playerControl, RoleTypes roleTypes, bool showIntro)
        {
            playerControl?.Data?.Role.OnRoleSet();

            bool ghostRole = RoleManager.IsGhostRole(roleTypes);
            playerControl.roleAssigned = true;
            int attempts = 0;
            while ((!playerControl.Data || GameManager.Instance == null || !GameManager.Instance) && attempts < 60)
            {
                attempts++;
                yield return Effects.Wait(0.1f);
            }
            if (!playerControl.Data)
            {
                Debug.LogWarning("CoSetRole timed out waiting for NetworkedPlayerInfo");
                yield break;
            }
            if (GameManager.Instance == null || !GameManager.Instance)
            {
                Debug.LogWarning("CoSetRole timed out waiting for GameManager");
                yield break;
            }
            if (ghostRole)
            {
                DestroyableSingleton<RoleManager>.Instance.SetRole(playerControl, roleTypes);
                playerControl.Data.Role.SpawnTaskHeader(playerControl);
                if (playerControl.AmOwner)
                {
                    DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(false);
                }
            }
            else
            {
                playerControl.RemainingEmergencies = GameManager.Instance.LogicOptions.GetNumEmergencyMeetings();
                DestroyableSingleton<RoleManager>.Instance.SetRole(playerControl, roleTypes);
                playerControl.Data.Role.SpawnTaskHeader(playerControl);
                playerControl.MyPhysics.SetBodyType(playerControl.BodyType);
                if (playerControl.AmOwner)
                {
                    if (playerControl.Data.Role.IsImpostor)
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.GamesAsImpostor);
                        DataManager.Player.Stats.ResetStat(StatID.CrewmateStreak);
                    }
                    else
                    {
                        DataManager.Player.Stats.IncrementStat(StatID.GamesAsCrewmate);
                        DataManager.Player.Stats.IncrementStat(StatID.CrewmateStreak);
                    }
                    DestroyableSingleton<HudManager>.Instance.MapButton.gameObject.SetActive(true);
                    DestroyableSingleton<HudManager>.Instance.ReportButton.gameObject.SetActive(true);
                    DestroyableSingleton<HudManager>.Instance.UseButton.gameObject.SetActive(true);
                    PlayerControl.AllPlayerControls.ForEach(new Action<PlayerControl>(delegate (PlayerControl pc)
                    {
                        if (pc.Data != null && !pc.Data.Disconnected)
                        {
                            PlayerNameColor.Set(pc);
                        }
                    }));
                }
            }
            if (showIntro && playerControl.AmOwner)
            {
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
                DestroyableSingleton<HudManager>.Instance.HideGameLoader();
            }
            if (!ghostRole)
            {
                playerControl.StopAllCoroutines();
            }
        }
    }
}
