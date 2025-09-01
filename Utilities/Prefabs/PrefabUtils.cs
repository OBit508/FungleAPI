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

namespace FungleAPI.Utilities.Prefab
{
    public static class PrefabUtils
    {
        public static bool AnyModUseShipPrefabs;
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
                            yield return new WaitForSeconds(1);
                        }
                    }
                    switch (shipRef.Asset.name)
                    {
                        case "SkeldShip": SkeldPrefab = shipRef.Asset.SafeCast<SkeldShipStatus>(); break;
                        case "MiraShip": MiraPrefab = shipRef.Asset.SafeCast<MiraShipStatus>(); break;
                        case "PolusShip": PolusPrefab = shipRef.Asset.SafeCast<PolusShipStatus>(); break;
                        case "Airship": AirshipPrefab = shipRef.Asset.SafeCast<AirshipStatus>(); break;
                        case "FungleShip": FunglePrefab = shipRef.Asset.SafeCast<FungleShipStatus>(); break;
                    }
                }
            }
        }
        public static T Prefab<T>(int index = 0) where T : UnityEngine.Object
        {
            return Resources.FindObjectsOfTypeAll(Il2CppType.From(typeof(T)))[index].SafeCast<T>();
        }
        public static SkeldShipStatus SkeldPrefab;
        public static MiraShipStatus MiraPrefab;
        public static PolusShipStatus PolusPrefab;
        public static AirshipStatus AirshipPrefab;
        public static FungleShipStatus FunglePrefab;
    }
}
