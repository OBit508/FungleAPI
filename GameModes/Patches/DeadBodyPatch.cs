using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    internal static class DeadBodyPatch
    {
        public static bool Prefix()
        {
            if (!GameManager.Instance.IsHideAndSeek() && !GameModeManager.GetCurrentGameMode().CanReportBodies()) return false;
            return true;
        }
    }
}
