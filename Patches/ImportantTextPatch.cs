using FungleAPI.Role;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(ImportantTextTask), "AppendTaskText")]
    internal static class ImportantTextPatch
    {
        public static bool Prefix(ImportantTextTask __instance, [HarmonyArgument(0)] Il2CppSystem.Text.StringBuilder sb)
        {
            if (!PlayerControl.LocalPlayer.Data.Role.CanDoTasks())
            {
                sb.AppendLine("<color=#" + ColorUtility.ToHtmlStringRGBA(PlayerControl.LocalPlayer.Data.Role.TeamColor) + ">" + TranslationController.Instance.GetString(StringNames.FakeTasks) + "</color>");
            }
            return false;
        }
    }
}
