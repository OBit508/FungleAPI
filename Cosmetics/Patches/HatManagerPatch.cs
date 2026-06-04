using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics.Patches
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
    internal static class HatManagerPatch
    {
        public static void Postfix(HatManager __instance)
        {
            SetHat(__instance);
            SetSkin(__instance);
            SetVisor(__instance);
            SetPet(__instance);
            SetNamePlate(__instance);
        }
        public static void SetHat(HatManager hatManager)
        {
            HatData empty = hatManager.allHats.First();
            IEnumerable<HatData> vanilla = hatManager.allHats.Where(h => !h.IsEmpty);

            hatManager.allHats = null;

            List<HatData> cosmetics = new List<HatData>() { empty };

            foreach (BaseCosmetic cosmetic in CosmeticManager.AllHats)
            {
                cosmetics.Add(cosmetic.Data.SafeCast<HatData>());
            
            }
            hatManager.allHats = cosmetics.ToArray();
            cosmetics = null;

            hatManager.allHats = hatManager.allHats.Concat(vanilla).ToArray();
        }
        public static void SetSkin(HatManager hatManager)
        {
            SkinData empty = hatManager.allSkins.First();
            IEnumerable<SkinData> vanilla = hatManager.allSkins.Where(h => !h.IsEmpty);

            hatManager.allSkins = null;

            List<SkinData> cosmetics = new List<SkinData>() { empty };

            foreach (BaseCosmetic cosmetic in CosmeticManager.AllSkins)
            {
                cosmetics.Add(cosmetic.Data.SafeCast<SkinData>());

            }

            hatManager.allSkins = cosmetics.ToArray();
            cosmetics = null;

            hatManager.allSkins = hatManager.allSkins.Concat(vanilla).ToArray();
        }
        public static void SetVisor(HatManager hatManager)
        {
            VisorData empty = hatManager.allVisors.First();
            IEnumerable<VisorData> vanilla = hatManager.allVisors.Where(h => !h.IsEmpty);

            hatManager.allVisors = null;

            List<VisorData> cosmetics = new List<VisorData>() { empty };

            foreach (BaseCosmetic cosmetic in CosmeticManager.AllVisors)
            {
                cosmetics.Add(cosmetic.Data.SafeCast<VisorData>());

            }

            hatManager.allVisors = cosmetics.ToArray();
            cosmetics = null;

            hatManager.allVisors = hatManager.allVisors.Concat(vanilla).ToArray();
        }
        public static void SetPet(HatManager hatManager)
        {
            PetData empty = hatManager.allPets.First();
            IEnumerable<PetData> vanilla = hatManager.allPets.Where(h => !h.IsEmpty);

            hatManager.allPets = null;

            List<PetData> cosmetics = new List<PetData>() { empty };

            foreach (BaseCosmetic cosmetic in CosmeticManager.AllPets)
            {
                cosmetics.Add(cosmetic.Data.SafeCast<PetData>());

            }

            hatManager.allPets = cosmetics.ToArray();
            cosmetics = null;

            hatManager.allPets = hatManager.allPets.Concat(vanilla).ToArray();
        }
        public static void SetNamePlate(HatManager hatManager)
        {
            NamePlateData empty = hatManager.allNamePlates.First();
            IEnumerable<NamePlateData> vanilla = hatManager.allNamePlates.Where(h => !h.IsEmpty);

            hatManager.allNamePlates = null;

            List<NamePlateData> cosmetics = new List<NamePlateData>() { empty };

            foreach (BaseCosmetic cosmetic in CosmeticManager.AllHats)
            {
                cosmetics.Add(cosmetic.Data.SafeCast<NamePlateData>());

            }

            hatManager.allNamePlates = cosmetics.ToArray();
            cosmetics = null;

            hatManager.allNamePlates = hatManager.allNamePlates.Concat(vanilla).ToArray();
        }
    }
}
