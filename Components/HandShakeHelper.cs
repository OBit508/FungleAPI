using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.GameOptions;
using FungleAPI.GlobalPatches;
using FungleAPI.Networking;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [Attributes.RegisterTypeInIl2Cpp]
    public class HandShakeHelper : MonoBehaviour
    {
        public PlayerControl Owner;

        public List<BepInMod> SameMods = new List<BepInMod>();
        public List<BepInMod> MissingMods = new List<BepInMod>();
        public List<BepInMod> MissingOnClientMods = new List<BepInMod>();

        public bool DidHandShake;
        public System.Collections.IEnumerator ClientInitialize()
        {
            yield return Owner.ClientInitialize();

            if (!AmongUsClient.Instance.AmHost) yield break;

            if (AmongUsClient.Instance.HostId == Owner.OwnerId) yield break;

            if (!HandShakeManager.ModdedServerHandshakeActive.GetValueOrDefault())
            {
                ClientData clientData = AmongUsClient.Instance.GetClient(Owner.OwnerId);

                if (!DidHandShake)
                {
                    AmongUsClient.Instance.KickPlayer(Owner.OwnerId, false);
                    HudManager.Instance?.Notifier.AddDisconnectMessage(string.Format(FungleTranslation.HandShakeFail_MissingAPIDisconnect.GetString(), clientData.PlayerName));

                    yield break;
                }

                if (MissingMods.Count > 0 || MissingOnClientMods.Count > 0)
                {
                    string missingModsText = "";
                    string extraModsText = "";
                    if (MissingMods.Count > 0)
                    {
                        int i = 0;
                        foreach (BepInMod missingMod in MissingMods)
                        {
                            missingModsText += $"{missingMod.Name} v{missingMod.Version}";
                            i++;

                            if (MissingMods.Count > i)
                            {
                                missingModsText += ", ";
                            }
                            else
                            {
                                missingModsText += ".";
                            }
                        }
                    }
                    if (MissingOnClientMods.Count > 0)
                    {
                        int i = 0;
                        foreach (BepInMod extraMod in MissingOnClientMods)
                        {
                            extraModsText += $"{extraMod.Name} v{extraMod.Version}";
                            i++;

                            if (MissingOnClientMods.Count > i)
                            {
                                extraModsText += ", ";
                            }
                            else
                            {
                                extraModsText += ".";
                            }
                        }
                    }

                    Rpc<RpcSendModsDisconnect>.Instance.Send(new KeyValuePair<string, string>(missingModsText, extraModsText), PlayerControl.LocalPlayer);
                    AmongUsClient.Instance.KickPlayer(Owner.OwnerId, false);
                    HudManager.Instance?.Notifier.AddDisconnectMessage(string.Format(FungleTranslation.HandShakeFail_ModdedPlayerDisconnect.GetString(), clientData.PlayerName));

                    yield break;
                }
            }

            SyncManager.RpcSyncEverything(Owner.OwnerId);
        }
        public System.Collections.IEnumerator CoStartPlayer()
        {
            yield return Owner.AssertWithTimeout(new Func<bool>(() => Owner.PlayerId != byte.MaxValue), new Action(delegate
            {
                AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player ID assignment");
            }), 30f);
            yield return Owner.AssertWithTimeout(new Func<bool>(() => GameManager.Instance != null && GameData.Instance != null && Owner.Data != null), new Action(delegate
            {
                AmongUsClient.Instance.EnqueueDisconnect(DisconnectReasons.Error, "Timeout while waiting for player data containers");
            }), 30f);
            Owner.RemainingEmergencies = GameManager.Instance.LogicOptions.GetNumEmergencyMeetings();
            Owner.SetColorBlindTag();
            Owner.cosmetics.UpdateVisibility();
            if (Owner.AmOwner)
            {
                Owner.lightSource = GameObject.Instantiate<LightSource>(Owner.LightPrefab, base.transform, false);
                Owner.lightSource.Initialize(Owner.Collider.offset * 0.5f);
                PlayerControl.LocalPlayer = Owner;
                Owner.cosmetics.SetAsLocalPlayer();
                Camera.main.GetComponent<FollowerCamera>().SetTarget(Owner);
                Owner.SetName(DataManager.Player.Customization.Name);
                Owner.SetColor((int)DataManager.Player.Customization.Color);
                if (Application.targetFrameRate > 30)
                {
                    Owner.MyPhysics.EnableInterpolation();
                }

                if (!HandShakeManager.ModdedServerHandshakeActive.GetValueOrDefault())
                {
                    FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Sending mods");
                    Rpc<RpcSendMods>.Instance.SendLate(Owner);
                }

                Owner.CmdCheckName(DataManager.Player.Customization.Name);
                Owner.CmdCheckColor(DataManager.Player.Customization.Color);
                Owner.RpcSetPet(DataManager.Player.Customization.Pet);
                Owner.RpcSetHat(DataManager.Player.Customization.Hat);
                Owner.RpcSetSkin(DataManager.Player.Customization.Skin);
                if (DestroyableSingleton<HatManager>.Instance.GetHatById(DataManager.Player.Customization.Hat).BlocksVisors)
                {
                    DataManager.Player.Customization.Visor = "visor_EmptyVisor";
                }
                Owner.RpcSetVisor(DataManager.Player.Customization.Visor);
                Owner.RpcSetNamePlate(DataManager.Player.Customization.NamePlate);
                Owner.RpcSetLevel(DataManager.Player.Stats.Level);
                yield return null;
            }
            else
            {
                Owner.StartCoroutine(ClientInitialize().WrapToIl2Cpp());
            }
            if (!Owner.Data.Role)
            {
                Owner.Data.Role = GameObject.Instantiate<RoleBehaviour>(GameData.Instance.DefaultRole);
                Owner.Data.Role.Initialize(Owner);
            }
            Owner.MyPhysics.SetBodyType(Owner.BodyType);
            if (Owner.isNew)
            {
                Owner.isNew = false;
                Owner.StartCoroutine(Owner.MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance));
            }
            if (PlayerControl.LocalPlayer == Owner)
            {
                Owner.clickKillCollider.enabled = false;
            }
        }
    }
}
