using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using HarmonyLib;
using LibCpp2IL.Elf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.Cosmetics
{
    [HarmonyPatch(typeof(HatManager), "Initialize")]
    public static class HatManagerPatch
    {
        public static void Postfix(HatManager __instance)
        {
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                ModCosmetics cosmetics = (ModCosmetics)Activator.CreateInstance(plugin.Cosmetics);
                List<PetData> pets = new List<PetData>();
                foreach (CustomPet pet in cosmetics.CustomPets)
                {
                    pets.Add(pet.Data);
                }
                __instance.allPets = pets.Concat(__instance.allPets).ToArray();
            }
        }
        
    }
}
