using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.MonoBehaviours;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(Animator), "Update")]
    internal class AnimatorPatch
    {
        public static bool Prefix(Animator __instance)
        {
            bool flag = true;
            if (__instance.GetComponent<SpriteAnimator>() != null && __instance.GetComponent<SpriteAnimator>().canPlay)
            {
                flag = false;
            }
            __instance.enabled = flag;
            return flag;
        }
    }
}
