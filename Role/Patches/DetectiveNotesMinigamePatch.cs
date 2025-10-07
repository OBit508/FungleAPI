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
        public static Translator typeText;
        public static StringNames TypeHereText
        {
            get
            {
                if (typeText == null)
                {
                    typeText = new Translator("Type here who was the killer");
                    typeText.AddTranslation(SupportedLangs.Latam, "Escribe aquí quién fue el asesino");
                    typeText.AddTranslation(SupportedLangs.Brazilian, "Digite aqui quem foi o assassino");
                    typeText.AddTranslation(SupportedLangs.Portuguese, "Digite aqui quem foi o assassino");
                    typeText.AddTranslation(SupportedLangs.Korean, "여기에 살인자가 누구였는지 입력하세요");
                    typeText.AddTranslation(SupportedLangs.Russian, "Введите здесь, кто был убийцей");
                    typeText.AddTranslation(SupportedLangs.Dutch, "Typ hier wie de moordenaar was");
                    typeText.AddTranslation(SupportedLangs.Filipino, "I-type dito kung sino ang pumatay");
                    typeText.AddTranslation(SupportedLangs.French, "Tapez ici qui était le meurtrier");
                    typeText.AddTranslation(SupportedLangs.German, "Geben Sie hier ein, wer der Mörder war");
                    typeText.AddTranslation(SupportedLangs.Italian, "Digita qui chi è stato l'assassino");
                    typeText.AddTranslation(SupportedLangs.Japanese, "ここに誰が殺人者だったか入力してください");
                    typeText.AddTranslation(SupportedLangs.Spanish, "Escribe aquí quién fue el asesino");
                    typeText.AddTranslation(SupportedLangs.SChinese, "在此输入谁是凶手");
                    typeText.AddTranslation(SupportedLangs.TChinese, "在此輸入誰是兇手");
                    typeText.AddTranslation(SupportedLangs.Irish, "Clóscríobh anseo cé a bhí ina mharaitheoir");
                }
                return typeText.StringName;
            }
        }
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
            text = "<size=2.25>" + TypeHereText.GetString() + "</size>";
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
                Player player = ReInput.players.GetPlayer(0);
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
