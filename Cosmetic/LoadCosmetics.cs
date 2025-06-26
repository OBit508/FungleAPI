using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Patches;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using FungleAPI.MonoBehaviours;

namespace FungleAPI.Cosmetic
{
    class LoadCosmetics
    {
        public static void Load()
        {
            new CustomColor(new Color32(29, 29, 29, byte.MaxValue), new Color32(8, 8, 8, byte.MaxValue), "Escuridão");
            Sprite sprite = ResourceHelper.LoadSprite("FungleAPI.Resources.fungleAPIHat", 100, Assembly.GetExecutingAssembly());
            new CustomHat("hat_FungleAPI", "Fungle API - Hat", sprite);
            new CustomVisor("visor_FungleAPI", "Fungle API - Visor", sprite);
        }
    }
}
