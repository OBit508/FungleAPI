using FungleAPI.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FungleAPI.Cosmetics
{
    public class ModCosmetics
    {
        public virtual (Color32 Color, Color32 ShadowColor, StringNames ColorName)[] CustomColors => new (Color32 Color, Color32 ShadowColor, StringNames ColorName)[] { };
        public virtual (string HatId, StringNames HatName, Sprite Sprite)[] CustomHats => new (string HatId, StringNames HatName, Sprite Sprite)[] { };
        public virtual (string VisorId, StringNames VisorName, Sprite Sprite)[] CustomVisors => new (string VisorId, StringNames VisorName, Sprite Sprite)[] { };
        public virtual CustomPet[] CustomPets => new CustomPet[] { };
        public static CustomPet GetCustomPet(string Id)
        {
            foreach (CustomPet pet in CustomPet.Pets)
            {
                if (pet.Data.ProductId == Id)
                {
                    return pet;
                }
            }
            return null;
        }
    }
}
