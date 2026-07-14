using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Role.Patches;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Hud.Patches
{
    [HarmonyPatch(typeof(HudManager._CoShowIntro_d__89), nameof(HudManager._CoShowIntro_d__89.MoveNext))]
    internal static class CoShowIntroPatch
    {
        public static bool Prefix(HudManager._CoShowIntro_d__89 __instance, ref bool __result)
        {
            if (!GameManager.Instance.IsHideAndSeek())
            {
                __instance.__4__this.StartCoroutine(CoShowIntro(__instance.__4__this).WrapToIl2Cpp());
                __result = false;
                return false;
            }
            return true;
        }
        public static System.Collections.IEnumerator CoShowIntro(HudManager hudManager)
        {
            while (!ShipStatus.Instance)
            {
                yield return null;
            }
            hudManager.IsIntroDisplayed = true;
            hudManager.LobbyTimerExtensionUI.HideAll();
            hudManager.SetMapButtonEnabled(false);
            DestroyableSingleton<HudManager>.Instance.FullScreen.transform.localPosition = new Vector3(0f, 0f, -250f);
            yield return DestroyableSingleton<HudManager>.Instance.ShowEmblem(true);
            IntroCutscene introCutscene = GameObject.Instantiate<IntroCutscene>(hudManager.IntroPrefab, hudManager.transform);
            yield return IntroCutscenePatch.CoBegin(introCutscene);
            PlayerControl.LocalPlayer.SetKillTimer(10f);
            ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().SetInitialSabotageCooldown();
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out systemType) && systemType.Is(out IDoorSystem doorSystem))
            {
                doorSystem.SetInitialSabotageCooldown();
            }
            yield return ShipStatus.Instance.PrespawnStep();
            PlayerControl.LocalPlayer.AdjustLighting();
            yield return hudManager.CoFadeFullScreen(Color.black, Color.clear, 0.2f, false);
            hudManager.FullScreen.transform.localPosition = new Vector3(0f, 0f, -500f);
            hudManager.IsIntroDisplayed = false;
            hudManager.SetMapButtonEnabled(true);
            hudManager.CrewmatesKilled.gameObject.SetActive(GameManager.Instance.ShowCrewmatesKilled());
            GameManager.Instance.StartGame();
            yield break;
        }
    }
}
