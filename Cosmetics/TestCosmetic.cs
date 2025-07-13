using FungleAPI.Assets;
using FungleAPI.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Cosmetics
{
    public class TestCosmetic : ModCosmetics
    {
        public static SpriteSheet anim = ResourceHelper.ConvertToSpriteSheet(new UnityEngine.Sprite[] { RolesSettingMenuPatch.Cog }, 0.1f);
        public override CustomPet[] CustomPets => new CustomPet[] {new CustomPet("testpet", StringNames.None, anim, anim, anim, anim, anim)};
    }
}
