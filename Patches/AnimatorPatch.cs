using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Animator), "Update")]
    internal class AnimatorPatch
    {
        public static bool Prefix(Animator __instance)
        {
            ChangeableValue<bool> flag;
            PlayerHelper.Animators.TryGetValue(__instance, out flag);
            bool update = true;
            if (flag != null && flag.Value)
            {
                update = false;
            }
            __instance.enabled = update;
            return update;
        }
    }
}
