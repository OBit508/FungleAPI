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
            SpriteAnimator animator = __instance.GetComponent<SpriteAnimator>();
            if (animator != null && animator.canPlay)
            {
                __instance.enabled = false;
                return false;
            }
            return true;
        }
    }
}
