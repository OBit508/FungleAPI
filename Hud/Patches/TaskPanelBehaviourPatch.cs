using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Utilities;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(TaskPanelBehaviour), "Update")] // MiraAPI Patch
    internal static class TaskPanelBehaviourPatch
    {
        public static bool Prefix(TaskPanelBehaviour __instance)
        {
            return __instance.name != "PlayerTab";
        }
    }
}
