using FungleAPI.MonoBehaviours;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ShipStatus), "Awake")]
    internal static class ShipStatusPatch
    {
        public static void Postfix()
        {
            VentHelper.ShipVents.Clear();
        }
    }
}
