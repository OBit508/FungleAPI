using FungleAPI.ModCompatibility;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FungleAPI.Utilities.Prefabs
{
    public static class PrefabUtils
    {
        private static Transform __prefabs;
        internal static Transform Prefabs
        {
            get
            {
                if (__prefabs == null)
                {
                    __prefabs = new GameObject("Prefabs").DontDestroy().transform;
                    __prefabs.gameObject.SetActive(false);
                }
                return __prefabs;
            }
        }
        internal static void ChangePrefab(AssetReference shipRef)
        {
            ShipStatus ship = shipRef.Asset.SafeCast<GameObject>().GetComponent<ShipStatus>();
            if (ship.SafeCast<SkeldShipStatus>() != null)
            {
                SkeldPrefab = ship.SafeCast<SkeldShipStatus>();
                FungleAPIPlugin.Instance.Log.LogInfo("Loaded SkeldPrefab");
            }
            if (ship.SafeCast<MiraShipStatus>() != null)
            {
                MiraPrefab = ship.SafeCast<MiraShipStatus>();
                FungleAPIPlugin.Instance.Log.LogInfo("Loaded MiraPrefab");
            }
            if (ship.SafeCast<PolusShipStatus>() != null)
            {
                PolusPrefab = ship.SafeCast<PolusShipStatus>();
                FungleAPIPlugin.Instance.Log.LogInfo("Loaded PolusPrefab");
            }
            if (ship.SafeCast<AirshipStatus>() != null)
            {
                AirshipPrefab = ship.SafeCast<AirshipStatus>();
                FungleAPIPlugin.Instance.Log.LogInfo("Loaded AirshipPrefab");
            }
            if (ship.SafeCast<FungleShipStatus>() != null)
            {
                FunglePrefab = ship.SafeCast<FungleShipStatus>();
                FungleAPIPlugin.Instance.Log.LogInfo("Loaded FunglePrefab");
            }
            ship.AllStepWatchers = Enumerable.ToArray<IStepWatcher>(Enumerable.OrderByDescending<IStepWatcher, int>(ship.GetComponentsInChildren<IStepWatcher>(), (IStepWatcher s) => s.Priority));
            ship.AllRooms = ship.GetComponentsInChildren<PlainShipRoom>().ToArray();
            ship.FastRooms = Enumerable.ToDictionary<PlainShipRoom, SystemTypes>(Enumerable.Where<PlainShipRoom>(ship.AllRooms, (PlainShipRoom p) => p.RoomId > SystemTypes.Hallway), (PlainShipRoom d) => d.RoomId).ToIl2CppDictionary();
            ship.AllCameras = ship.GetComponentsInChildren<SurvCamera>().ToArray();
            ship.AllConsoles = ship.GetComponentsInChildren<Console>().ToArray();
            ship.AllVents = ship.GetComponentsInChildren<Vent>().ToArray();
            ship.Ladders = ship.GetComponentsInChildren<Ladder>().ToArray();
        }
        internal static System.Collections.IEnumerator CoLoadShipPrefabs()
        {
            if (LevelImpostorUtils.GetLevelImpostor() == null)
            {
                for (int i = 0; i < AmongUsClient.Instance.ShipPrefabs.Count; i++)
                {
                    AssetReference shipRef = AmongUsClient.Instance.ShipPrefabs[i];
                    while (shipRef.Asset == null && shipRef.AssetGUID != "Submerged" && shipRef.AssetGUID != "AprilShip")
                    {
                        AsyncOperationHandle op = shipRef.LoadAssetAsync<GameObject>();
                        while (op.Status == AsyncOperationStatus.None)
                        {
                            yield return null;
                        }
                        if (op.Status == AsyncOperationStatus.Succeeded)
                        {
                            ChangePrefab(shipRef);
                        }
                    }
                }
            }
            else
            {
                while (!SkeldPrefab && !MiraPrefab && !PolusPrefab & !AirshipPrefab && !FunglePrefab)
                {
                    foreach (AssetReference shipRef in AmongUsClient.Instance.ShipPrefabs)
                    {
                        ChangePrefab(shipRef);
                    }
                    yield return new WaitForSeconds(1);
                }
            }
        } 
        public static T Prefab<T>(Predicate<T> predicate = null) where T : UnityEngine.Object
        {
            if (predicate == null)
            {
                predicate = new Predicate<T>(x => x);
            }
            return Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(T))).FirstOrDefault(x => predicate(x.SafeCast<T>())).SafeCast<T>();
        }
        public static SkeldShipStatus SkeldPrefab;
        public static MiraShipStatus MiraPrefab;
        public static PolusShipStatus PolusPrefab;
        public static AirshipStatus AirshipPrefab;
        public static FungleShipStatus FunglePrefab;
    }
}
