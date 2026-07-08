using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(PhantomRole), nameof(PhantomRole.CanUse))]
    internal static class PhantomRolePatch
    {
        public static bool Prefix(PhantomRole __instance, IUsable usable, ref bool __result)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            if (!GameModeManager.GetCurrentGameMode().CanUse(usable, __instance.Player))
            {
                __result = false;
                return false;
            }
            if (usable.SafeCast<Vent>() || usable.SafeCast<ZiplineConsole>() || usable.SafeCast<Ladder>() || usable.SafeCast<PlatformConsole>())
            {
                __result = !__instance.fading;
                return false;
            }
            if (__instance.isInvisible)
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
