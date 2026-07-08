using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.CanUse))]
    internal static class ImpostorRolePatch
    {
        public static bool Prefix(ImpostorRole __instance, IUsable usable, ref bool __result)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            if (!GameModeManager.GetCurrentGameMode().CanUse(usable, __instance.Player))
            {
                __result = false;
                return false;
            }
            Console console = usable.SafeCast<Console>();
            __result = !(console != null) || console.AllowImpostor;
            return false;
        }
    }
}
