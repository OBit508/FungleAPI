using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.GameModes;
using FungleAPI.Networking;
using FungleAPI.Player.Networking.Data;
using FungleAPI.Utilities;
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
            data.Source.StartCoroutine(CoSetRole(data.Source, data.RoleType, data.ShowIntro).WrapToIl2Cpp());
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
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(CoShowIntro().WrapToIl2Cpp());
                DestroyableSingleton<HudManager>.Instance.HideGameLoader();
            }
            if (!ghostRole)
            {
                playerControl.StopAllCoroutines();
            }
        }
        public System.Collections.IEnumerator CoShowIntro()
        {
            HudManager hudManager = HudManager.Instance;
            while (!ShipStatus.Instance)
            {
                yield return null;
            }
            hudManager.IsIntroDisplayed = true;
            hudManager.LobbyTimerExtensionUI.HideAll();
            hudManager.SetMapAndInfoButtonsEnabled(false);
            DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
            yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(true);
            IntroCutscene introCutscene = GameObject.Instantiate<IntroCutscene>(hudManager.IntroPrefab, hudManager.transform);
            yield return GameModeManager.GetCurrentGameMode().CoIntroBegin(introCutscene);
            PlayerControl.LocalPlayer.SetKillTimer(10f);
            ShipStatus.Instance.Systems[SystemTypes.Sabotage].SafeCast<SabotageSystemType>().SetInitialSabotageCooldown();
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out systemType) && systemType.Is(out IDoorSystem doorSystem))
            {
                doorSystem.SetInitialSabotageCooldown();
            }
            yield return ShipStatus.Instance.PrespawnStep();
            PlayerControl.LocalPlayer.AdjustLighting();
            yield return hudManager.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false);
            hudManager.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
            hudManager.IsIntroDisplayed = false;
            hudManager.SetMapAndInfoButtonsEnabled(true);
            hudManager.CrewmatesKilled.gameObject.SetActive(GameManager.Instance.ShowCrewmatesKilled());
            GameManager.Instance.StartGame();
            yield break;
        }
    }
}
