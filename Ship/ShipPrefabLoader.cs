using FungleAPI.Extensions;
using FungleAPI.ModCompatibility;
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
        public static SkeldShipStatus SkeldPrefab { get; internal set; }
        /// <summary>
        /// Cached Mira ship prefab
        /// </summary>
        public static MiraShipStatus MiraPrefab { get; internal set; }
        /// <summary>
        /// Cached Polus ship prefab
        /// </summary>
        public static PolusShipStatus PolusPrefab { get; internal set; }
        /// <summary>
        /// Cached Airship prefab
        /// </summary>
        public static AirshipStatus AirshipPrefab { get; internal set; }
        /// <summary>
        /// Cached Fungle ship prefab
        /// </summary>
        public static FungleShipStatus FunglePrefab { get; internal set; }

        public static ShipType ShipLoadFlag = ShipType.Skeld;

        internal static void ChangePrefab(AssetReference shipRef)
        {
            if (shipRef.Asset.Is(out GameObject shipObj))
            {
                ShipStatus ship = shipObj.GetComponent<ShipStatus>();
                if (ship.Is(out SkeldShipStatus skeldShipStatus))
                {
                    FungleAPIPlugin.Instance.Log.LogInfo("Loaded Skeld prefab");
                    SkeldPrefab = skeldShipStatus;
                }
                if (ship.Is(out MiraShipStatus miraShipStatus))
                {
                    FungleAPIPlugin.Instance.Log.LogInfo("Loaded Mira prefab");
                    MiraPrefab = miraShipStatus;
                }
                if (ship.Is(out PolusShipStatus polusShipStatus))
                {
                    FungleAPIPlugin.Instance.Log.LogInfo("Loaded Polus prefab");
                    PolusPrefab = polusShipStatus;
                }
                if (ship.Is(out AirshipStatus airshipStatus))
                {
                    FungleAPIPlugin.Instance.Log.LogInfo("Loaded Airship prefab");
                    AirshipPrefab = airshipStatus;
                }
                if (ship.Is(out FungleShipStatus fungleShipStatus))
                {
                    FungleAPIPlugin.Instance.Log.LogInfo("Loaded Fungle prefab");
                    FunglePrefab = fungleShipStatus;
                }
                StartShip(ship);
            }
        }
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
            if (LevelImpostorSupport.LevelImpostorAssembly == null)
            {
                ChangeableValue<bool> done = new ChangeableValue<bool>(false);
                System.Collections.IEnumerator WaitFor()
                {
                    AmongUsClient amongUsClient = AmongUsClient.Instance;
                    if (ShipLoadFlag.HasFlag(ShipType.Skeld))
                    {
                        AssetReference assetReference = amongUsClient.ShipPrefabs[SkeldID];
                        yield return CoLoad(assetReference);
                        ChangePrefab(assetReference);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.MiraHQ))
                    {
                        AssetReference assetReference = amongUsClient.ShipPrefabs[MiraID];
                        yield return CoLoad(assetReference);
                        ChangePrefab(assetReference);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Polus))
                    {
                        AssetReference assetReference = amongUsClient.ShipPrefabs[PolusID];
                        yield return CoLoad(assetReference);
                        ChangePrefab(assetReference);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Airship))
                    {
                        AssetReference assetReference = amongUsClient.ShipPrefabs[AirshipID];
                        yield return CoLoad(assetReference);
                        ChangePrefab(assetReference);
                    }
                    if (ShipLoadFlag.HasFlag(ShipType.Fungle))
                    {
                        AssetReference assetReference = amongUsClient.ShipPrefabs[FungleID];
                        yield return CoLoad(assetReference);
                        ChangePrefab(assetReference);
                    }
                    done.Value = true;
                }
                Helpers.StartCoroutine(WaitFor());
                yield return CoAnimateDots(textMeshPro, baseText, done);
            }
            else
            {
                ChangeableValue<bool> done = new ChangeableValue<bool>(false);
                System.Collections.IEnumerator WaitFor()
                {
                    yield return LevelImpostorSupport.CoUnsafeWaitForMapLoading();
                    done.Value = true;
                }
                Helpers.StartCoroutine(WaitFor());
                yield return CoAnimateDots(textMeshPro, baseText, done);
                foreach (AssetReference shipRef in AmongUsClient.Instance.ShipPrefabs)
                {
                    if (shipRef.Asset != null)
                    {
                        ChangePrefab(shipRef);
                    }
                }
            }
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
        internal static System.Collections.IEnumerator CoAnimateDots(TextMeshPro textMeshPro, string baseText, ChangeableValue<bool> func)
        {
            int dots = 0;
            float timer = 0f;
            while (!func.Value)
            {
                timer += Time.deltaTime;
                if (timer >= 0.35f)
                {
                    timer = 0f;
                    dots = (dots % 3) + 1;
                    textMeshPro.text = baseText + new string('.', dots) + "</font>";
                }
                yield return null;
            }
        }
    }
}
