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
        internal static void ChangePrefab(AssetReference shipRef)
        {
            switch (shipRef.Asset.name)
            {
                case "SkeldShip": SkeldPrefab = shipRef.Asset.SafeCast<SkeldShipStatus>(); break;
                case "MiraShip": MiraPrefab = shipRef.Asset.SafeCast<MiraShipStatus>(); break;
                case "PolusShip": PolusPrefab = shipRef.Asset.SafeCast<PolusShipStatus>(); break;
                case "Airship": AirshipPrefab = shipRef.Asset.SafeCast<AirshipStatus>(); break;
                case "FungleShip": FunglePrefab = shipRef.Asset.SafeCast<FungleShipStatus>(); break;
            }
        }
        public static System.Collections.IEnumerator CoLoadShipPrefabs()
        {
            if (LevelImpostorUtils.GetLevelImpostor() == null)
            {
                for (int i = 0; i < AmongUsClient.Instance.ShipPrefabs.Count; i++)
                {
                    AssetReference shipRef = AmongUsClient.Instance.ShipPrefabs[i];
                    while (shipRef.Asset == null && shipRef.AssetGUID != "Submerged" && shipRef.AssetGUID != "AprilShip")
                    {
                        AsyncOperationHandle op = shipRef.LoadAssetAsync<GameObject>();
                        if (!op.IsValid())
                        {
                            yield return new WaitForSeconds(3);
                        }
                    }
                    ChangePrefab(shipRef);
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
