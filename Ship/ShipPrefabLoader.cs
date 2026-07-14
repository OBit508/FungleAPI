using FungleAPI.Api;
using FungleAPI.Extensions;
using FungleAPI.ModCompatibility;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FungleAPI.Ship
{
    public static class ShipPrefabLoader
    {
        public static int SkeldID = 0;
        public static int MiraID = 1;
        public static int PolusID = 2;
        public static int AirshipID = 4;
        public static int FungleID = 5;

        /// <summary>
        /// Cached Skeld ship prefab
        /// </summary>
        public static SkeldShipStatus SkeldPrefab => AmongUsClient.Instance.ShipPrefabs[SkeldID].Asset?.Cast<GameObject>().GetComponent<SkeldShipStatus>();
        /// <summary>
        /// Cached Mira ship prefab
        /// </summary>
        public static MiraShipStatus MiraPrefab => AmongUsClient.Instance.ShipPrefabs[MiraID].Asset?.Cast<GameObject>().GetComponent<MiraShipStatus>();
        /// <summary>
        /// Cached Polus ship prefab
        /// </summary>
        public static PolusShipStatus PolusPrefab => AmongUsClient.Instance.ShipPrefabs[PolusID].Asset?.Cast<GameObject>().GetComponent<PolusShipStatus>();
        /// <summary>
        /// Cached Airship prefab
        /// </summary>
        public static AirshipStatus AirshipPrefab => AmongUsClient.Instance.ShipPrefabs[AirshipID].Asset?.Cast<GameObject>().GetComponent<AirshipStatus>();
        /// <summary>
        /// Cached Fungle ship prefab
        /// </summary>
        public static FungleShipStatus FunglePrefab => AmongUsClient.Instance.ShipPrefabs[FungleID].Asset?.Cast<GameObject>().GetComponent<FungleShipStatus>();

        public static ShipType ShipLoadFlag = ShipType.Skeld;
        internal static void StartShip(ShipStatus shipStatus)
        {
            shipStatus.AllStepWatchers = Enumerable.ToArray<IStepWatcher>(Enumerable.OrderByDescending<IStepWatcher, int>(shipStatus.GetComponentsInChildren<IStepWatcher>(), (IStepWatcher s) => s.Priority));
            shipStatus.AllRooms = shipStatus.GetComponentsInChildren<PlainShipRoom>().ToArray();
            shipStatus.FastRooms = Enumerable.ToDictionary<PlainShipRoom, SystemTypes>(Enumerable.Where<PlainShipRoom>(shipStatus.AllRooms, (PlainShipRoom p) => p.RoomId > SystemTypes.Hallway), (PlainShipRoom d) => d.RoomId).ToIl2CppDictionary();
            shipStatus.AllCameras = shipStatus.GetComponentsInChildren<SurvCamera>().ToArray();
            shipStatus.AllConsoles = shipStatus.GetComponentsInChildren<Console>().ToArray();
            shipStatus.AllVents = shipStatus.GetComponentsInChildren<Vent>().ToArray();
            shipStatus.Ladders = shipStatus.GetComponentsInChildren<Ladder>().ToArray();
        }
        internal static System.Collections.IEnumerator CoLoadShipPrefabs(TextMeshPro textMeshPro, string baseText)
        {
            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Loading ship prefabs...");
            if (LevelImpostorSupport.LevelImpostorAssembly == null)
            {
                ChangeableValue<bool> done = new ChangeableValue<bool>(false);
                System.Collections.IEnumerator WaitFor()
                {
                    AmongUsClient amongUsClient = AmongUsClient.Instance;

                    if (SubmergedCompatibility.Instance != null)
                    {
                        FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Waiting for submerged...");
                        yield return SubmergedCompatibility.Instance.CoWaitMapLoader();
                    }

                    if (ShipLoadFlag.HasFlag(ShipType.Skeld) || SubmergedCompatibility.Instance != null)
                    {
                        yield return CoLoadAndChange(amongUsClient.ShipPrefabs[SkeldID]);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.MiraHQ))
                    {
                        yield return CoLoadAndChange(amongUsClient.ShipPrefabs[MiraID]);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Polus))
                    {
                        yield return CoLoadAndChange(amongUsClient.ShipPrefabs[PolusID]);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Airship) || SubmergedCompatibility.Instance != null)
                    {
                        yield return CoLoadAndChange(amongUsClient.ShipPrefabs[AirshipID]);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Fungle) || SubmergedCompatibility.Instance != null)
                    {
                        yield return CoLoadAndChange(amongUsClient.ShipPrefabs[FungleID]);
                    }
                    done.Value = true;
                }
                Helpers.StartCoroutine(WaitFor());
                yield return FungleApiPlugin.CoAnimateDots(textMeshPro, baseText, done);
            }
            else
            {
                ChangeableValue<bool> done = new ChangeableValue<bool>(false);
                System.Collections.IEnumerator WaitFor()
                {
                    FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Waiting for LevelImpostor...");
                    yield return LevelImpostorSupport.CoWaitMapLoading();
                    done.Value = true;
                }
                Helpers.StartCoroutine(WaitFor());
                yield return FungleApiPlugin.CoAnimateDots(textMeshPro, baseText, done);
            }
        }
        internal static System.Collections.IEnumerator CoLoadAndChange(AssetReference assetReference)
        {
            if (assetReference.Asset == null) yield return CoLoad(assetReference);
        }
        internal static System.Collections.IEnumerator CoLoad(AssetReference assetReference)
        {
            if (assetReference.Asset != null)
            {
                yield break;
            }
            AsyncOperationHandle<GameObject> op = assetReference.LoadAssetAsync<GameObject>();
            if (!op.IsValid())
            {
                yield break;
            }
            yield return op;
            if (op.Status != AsyncOperationStatus.Succeeded)
            {
                yield break;
            }
        }
        
    }
}
