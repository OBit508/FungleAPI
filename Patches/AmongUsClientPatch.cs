using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(AmongUsClient), "Awake")]
    internal static class AmongUsClientPatch
    {
        public static void Postfix(AmongUsClient __instance)
        {
            bool flag = false;
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                if (plugin.UseShipReference)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                __instance.StartCoroutine(Utilities.Prefabs.PrefabUtils.CoLoadShipPrefabs().WrapToIl2Cpp());
            }
        }
    }
}
