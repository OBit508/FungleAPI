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
            PlayerHelper helper;
            if (PlayerHelper.Animators.TryGetValue(__instance, out helper) && helper.Current != null)
            {
                helper.Group.SpriteAnimator.m_nodes.m_spriteRenderer.sprite = helper.Current.Sprites[helper.currentSprite];
                return false;
            }
            return true;
        }
    }
}
