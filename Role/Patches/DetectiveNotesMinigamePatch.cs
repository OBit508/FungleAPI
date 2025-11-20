using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Patches
{
    [HarmonyPatch(typeof(DetectiveNotesMinigame))]
    internal static class DetectiveNotesMinigamePatch
    {
        public static string text;
        public static bool typing;
        public static TouchScreenKeyboard keyboard;
        public static SpriteRenderer Pencil;
        [HarmonyPatch("Begin")]
        [HarmonyPostfix]
        public static void BeginPostfix(DetectiveNotesMinigame __instance)
        {
            keyboard = null;
            typing = false;
            text = "<size=2.25>" + FungleTranslation.TypeHereText.GetString() + "</size>";
            Pencil = __instance.ControllerSelectable[2].GetComponent<SpriteRenderer>();
            __instance.ControllerSelectable[2].SafeCast<PassiveButton>().SetNewAction(new Action(delegate
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    keyboard = TouchScreenKeyboard.Open(__instance.impostorTypeText.text == text && !__instance.associatedDetective.notesPageInfos[__instance.currentPageIndex].impostorSet ? "" : __instance.impostorTypeText.text, TouchScreenKeyboardType.Default);
                }
                else
                {
                    typing = !typing;
                }
            }));
        }
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool UpdatePrefix(DetectiveNotesMinigame __instance)
        {
            if (ReInput.players.GetPlayer(0).GetButtonDown(49) && !typing)
            {
                __instance.Close();
            }
            if (ControllerManager.Instance.CurrentUiState.MenuName == __instance.MainOverlay)
            {
                Rewired.Player player = ReInput.players.GetPlayer(0);
                bool flag = false;
                int num = __instance.currentPageIndex;
                if (player.GetButtonDown(35))
                {
                    num = __instance.currentPageIndex - 1;
                    if (num < 0)
                    {
                        num = __instance.tabs.Count - 1;
                    }
                    flag = true;
                }
                if (player.GetButtonDown(34))
                {
                    num = __instance.currentPageIndex + 1;
                    if (num >= __instance.tabs.Count)
                    {
                        num = 0;
                    }
                    flag = true;
                }
                if (flag)
                {
                    __instance.SetPage(num);
                }
            }
            DetectiveNotesPageInfo pageInfo = __instance.associatedDetective.notesPageInfos[__instance.currentPageIndex];
            if (Pencil != null)
            {
                Pencil.color = typing ? Color.green : Color.white;
            }
            if (keyboard != null || typing)
            {
                if (__instance.impostorTypeText.text == text && !pageInfo.impostorSet)
                {
                    __instance.impostorTypeText.text = "";
                }
                if (keyboard != null && Application.platform == RuntimePlatform.Android)
                {
                    __instance.impostorTypeText.text = keyboard.text;
                }
                if (typing)
                {
                    string input = Input.inputString;
                    if (input != null && input.Length > 0)
                    {
                        if (input.Contains("\b"))
                        {
                            input = input.Replace("\b", "");
                            if (__instance.impostorTypeText.text.Length > 0)
                            {
                                __instance.impostorTypeText.text = __instance.impostorTypeText.text.Remove(__instance.impostorTypeText.text.Length - 1, 1);
                            }
                        }
                        __instance.impostorTypeText.text += input;
                    }
                }
                pageInfo.SetImpostorType(__instance.impostorTypeText.text);
            }
            else if (!pageInfo.impostorSet)
            {
                __instance.impostorTypeText.text = text;
            }
            return false;
        }
        [HarmonyPatch("SetUpCurrentPage")]
        [HarmonyPostfix]
        public static void SetPagePostfix()
        {
            keyboard = null;
            typing = false;
        }
    }
}
