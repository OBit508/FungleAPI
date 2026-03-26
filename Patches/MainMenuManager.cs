using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using Il2CppMono.Security.X509;
using Rewired.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager))]
    internal static class MainMenuManagerPatch
    {
        public static ModsPage modsPage;
        [HarmonyPatch("ResetScreen")]
        [HarmonyPostfix]
        public static void ResetScreenPostfix()
        {
            modsPage?.gameObject.SetActive(false);
        }
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(MainMenuManager __instance)
        {
            ChatLanguageSet.Instance.Load();
            __instance.StartCoroutine(RunStartUp(__instance));
            QualitySettings.vSyncCount = (DataManager.Settings.Video.VSync ? 1 : 0);
            __instance.announcementPopUp.ActionOnDisable += new Action(delegate
            {
                __instance.screenMask.enabled = true;
            });
            __instance.announcementPopUp.ActionOnEnable += new Action(delegate
            {
                __instance.screenMask.enabled = false;
            });
            __instance.announcementPopUp.SetMainMenuManager(__instance);
            __instance.findGameButton.GetComponent<PassiveButton>().SetNewAction(delegate
            {
                Helpers.ShowPopup(FungleTranslation.ChangeToPublicText.GetString());
            });
            foreach (SpriteRenderer spriteRenderer in __instance.findGameButton.GetComponentsInChildren<SpriteRenderer>(true))
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f;
                spriteRenderer.color = color;
            }
            AspectPosition follower = FungleAssets.ModsPagePrefab.Instantiate().AddComponent<AspectPosition>();
            follower.transform.parent = __instance.creditsScreen.transform.parent;
            follower.anchorPoint = new Vector2(0.45f, 0.5f);
            follower.DistanceFromEdge = new Vector3(0, 0, -10);
            follower.Alignment = AspectPosition.EdgeAlignments.Center;
            follower.updateAlways = true;
            modsPage = follower.GetComponent<ModsPage>();
            modsPage.gameObject.SetActive(false);
            AjustButtons(__instance, delegate
            {
                __instance.ResetScreen();
                modsPage?.gameObject.SetActive(true);
                __instance.screenTint.enabled = true;
            });
        }
        public static Transform AjustButtons(MainMenuManager mainMenuManager, Action mods)
        {
            AspectPosition exitGame = mainMenuManager.quitButton.GetComponent<AspectPosition>();
            AspectPosition credits = mainMenuManager.creditsButton.GetComponent<AspectPosition>();
            PassiveButton modsButton = GameObject.Instantiate<PassiveButton>(mainMenuManager.creditsButton, mainMenuManager.creditsButton.transform.parent);
            modsButton.SetNewAction(mods);
            TextMeshPro textMeshPro = modsButton.transform.GetChild(2).GetChild(0).GetComponent<TextMeshPro>();
            GameObject.Destroy(textMeshPro.GetComponent<TextTranslatorTMP>());
            textMeshPro.text = "Mods";
            AspectPosition aspectPosition = modsButton.GetComponent<AspectPosition>();
            credits.anchorPoint = new Vector2(0.391f, credits.anchorPoint.y);
            aspectPosition.anchorPoint = new Vector2(0.5005f, credits.anchorPoint.y);
            exitGame.anchorPoint = new Vector2(0.610f, credits.anchorPoint.y);
            foreach (AspectPosition aspect in new List<AspectPosition>() { exitGame, credits, aspectPosition })
            {
                AspectScaledAsset aspectScaledAsset = aspect.GetComponent<AspectScaledAsset>();
                foreach (AspectScaledAsset.ScaledSprite scaledSprite in aspectScaledAsset.allSprites)
                {
                    scaledSprite.OriginalWidth = 1.43f;
                }
                foreach (AspectScaledAsset.ScaledCollider scaledCollider in aspectScaledAsset.allColliders)
                {
                    scaledCollider.OriginalWidth = 1.42f;
                }
                aspectScaledAsset.ScaleObject(1);
                TextMeshPro text = aspectScaledAsset.transform.GetChild(2).GetChild(0).GetComponent<TextMeshPro>();
                text.alignment = TextAlignmentOptions.Center;
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.501f, 0.505f);
                text.transform.parent.localPosition = Vector3.zero;
            }
            return modsButton.transform;
        }
        public static System.Collections.IEnumerator RunStartUp(MainMenuManager mainMenuManager)
        {
            mainMenuManager.finishStartup = false;
            AccountManager.Instance.waitingText.gameObject.SetActive(true);
            AccountManager.Instance.waitingText.transform.position = new Vector3(0, 0, -25);
            TextMeshPro textMeshPro = new GameObject("MainScreen Load Text").AddComponent<TextMeshPro>();
            textMeshPro.gameObject.layer = 5;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMeshPro.enableWordWrapping = false;
            textMeshPro.fontSize = 4;
            textMeshPro.characterSpacing = 4;
            textMeshPro.transform.position = new Vector3(0, 0, -500);
            if (!ShipsLoaded)
            {
                string baseText = "<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">" + FungleTranslation.LoadingPrefabsText.GetString();
                yield return Utilities.Prefabs.PrefabUtils.CoLoadShipPrefabs(textMeshPro, baseText);
                ShipsLoaded = true;
            }
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                IFungleBasePlugin fungleBasePlugin = plugin.BasePlugin as IFungleBasePlugin;
                if (fungleBasePlugin != null)
                {
                    yield return fungleBasePlugin.CoLoadOnMainScreen(textMeshPro);
                }
            }
            GameObject.Destroy(textMeshPro.gameObject);
            yield return mainMenuManager.RunStartUp();
        }
        public static bool ShipsLoaded;
    }
}
