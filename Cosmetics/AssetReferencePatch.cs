using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using BepInEx.Unity.IL2CPP.Utils;

namespace FungleAPI.Cosmetics
{
    [HarmonyPatch(typeof(AssetReference))]
    public static class AssetReferencePatch
    {
        public static void Instantiate(AsyncOperationHandle<GameObject> operation, string id)
        {
            bool b = false;
            while (!b)
            {
                b = operation.Result != null;
            }
            CustomPet p = ModCosmetics.GetCustomPet(id);
            CustomPetBehaviour pet = operation.Result.AddComponent<CustomPetBehaviour>().DontDestroy();
            pet.Animator = SpriteAnimator.AddCustomAnimator(pet.GetComponent<SpriteRenderer>());
            Transform shadow = pet.Pet.shadows[0].transform;
            shadow.localPosition = p.ShadowPosition;
            shadow.localScale = p.ShadowScale;
            pet.Data = p;
            pet.Pet.data = p.Data;
        }
        [HarmonyPatch("Instantiate", new Type[] {typeof(Transform), typeof(bool)})]
        [HarmonyPostfix]
        public static void Instantiate1(AssetReference __instance, [HarmonyArgument(0)] Transform parent, ref AsyncOperationHandle<GameObject> __result)
        {
            if (ModCosmetics.GetCustomPet(__instance.SubObjectName) != null)
            {
                Instantiate(__result, __instance.SubObjectName);
            }
        }
        [HarmonyPatch("Instantiate", new Type[] { typeof(Vector3), typeof(Quaternion), typeof(Transform) })]
        [HarmonyPostfix]
        public static void Instantiate2(AssetReference __instance, [HarmonyArgument(0)] Transform parent, ref AsyncOperationHandle<GameObject> __result)
        {
            if (ModCosmetics.GetCustomPet(__instance.SubObjectName) != null)
            {
                Instantiate(__result, __instance.SubObjectName);
            }
        }
    }
}
