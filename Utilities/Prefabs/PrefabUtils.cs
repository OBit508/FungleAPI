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
            if (shipRef.Asset.SafeCast<GameObject>().GetComponent<SkeldShipStatus>() != null)
            {
                SkeldPrefab = shipRef.Asset.SafeCast<GameObject>().GetComponent<SkeldShipStatus>();
            }
            if (shipRef.Asset.SafeCast<GameObject>().GetComponent<MiraShipStatus>() != null)
            {
                MiraPrefab = shipRef.Asset.SafeCast<GameObject>().GetComponent<MiraShipStatus>();
            }
            if (shipRef.Asset.SafeCast<GameObject>().GetComponent<PolusShipStatus>() != null)
            {
                PolusPrefab = shipRef.Asset.SafeCast<GameObject>().GetComponent<PolusShipStatus>();
            }
            if (shipRef.Asset.SafeCast<GameObject>().GetComponent<AirshipStatus>() != null)
            {
                AirshipPrefab = shipRef.Asset.SafeCast<GameObject>().GetComponent<AirshipStatus>();
            }
            if (shipRef.Asset.SafeCast<GameObject>().GetComponent<FungleShipStatus>() != null)
            {
                FunglePrefab = shipRef.Asset.SafeCast<GameObject>().GetComponent<FungleShipStatus>();
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
