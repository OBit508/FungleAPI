using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Components;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), "Start")]
    internal static class MainMenuManagerPatch
    {
        public static bool Prefix(MainMenuManager __instance)
        {
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
            SpriteRenderer background = GameObject.Instantiate<SpriteRenderer>(__instance.findGameButton.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>(), __instance.findGameButton.transform.GetChild(1).GetChild(0));
            background.color = Color.black;
            Vector3 pos = __instance.findGameButton.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).transform.position;
            pos.z += 0.1f;
            background.transform.position = pos;
            ChatLanguageSet.Instance.Load();
            __instance.StartCoroutine(RunStartUp(__instance).WrapToIl2Cpp());
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
            Transform parent = __instance.transform.GetChild(5).GetChild(1).GetChild(3);
            ModsPage page = FungleAssets.ModsPagePrefab.Instantiate(parent).GetComponent<ModsPage>();
            PassiveButton button = GameObject.Instantiate<PassiveButton>(__instance.creditsButton, parent);
            button.transform.localScale = Vector3.one * 1.1f;
            button.SetNewAction(delegate
            {
                page.Opening = !page.Opening;
            });
            button.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.2f, 0.89f);
            button.GetComponent<AspectPosition>().Update();
            TextMeshPro text = button.transform.GetChild(2).GetChild(0).GetComponent<TextMeshPro>();
            text.GetComponent<TextTranslatorTMP>().enabled = false;
            text.text = "Mods";
            page.Closed = button.transform.localPosition;
            page.transform.localScale = Vector3.zero;
            return false;
        }
        public static System.Collections.IEnumerator RunStartUp(MainMenuManager mainMenuManager)
        {
            mainMenuManager.finishStartup = false;
            Logger.GlobalInstance.Info("MainMenuManager.RunStartUp beginning", null);
            Constants.SetMainThread();
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
            DestroyableSingleton<EOSManager>.Instance.FinishedAssets = true;
            GameObject.Instantiate<HatManager>(mainMenuManager.HatManagerRef).Initialize();
            mainMenuManager.cosmicubeManager = GameObject.Instantiate<CosmicubeManager>(mainMenuManager.CosmicubeManagerRef);
            mainMenuManager.cosmicubeManager.Initialize();
            yield return DestroyableSingleton<EOSManager>.Instance.RunLogin();
            mainMenuManager.rightPanelMask.gameObject.SetActive(true);
            if (!DestroyableSingleton<StoreManager>.InstanceExists || !DestroyableSingleton<StoreManager>.Instance.FinishedInitializationFlow)
            {
                DestroyableSingleton<StoreManager>.Instance.Initialize();
            }
            if (!DestroyableSingleton<StoreMenu>.InstanceExists || !DestroyableSingleton<StoreMenu>.Instance.Initialized)
            {
                DestroyableSingleton<StoreMenu>.Instance.Initialize();
            }
            while (!DestroyableSingleton<EOSManager>.Instance.HasFinishedLoginFlow() || DestroyableSingleton<AccountManager>.Instance.signInScreen.IsOpen())
            {
                yield return new WaitForSeconds(0.3f);
            }
            if (DataManager.Player.Account.LoginStatus != EOSManager.AccountLoginStatus.Offline)
            {
                DestroyableSingleton<EOSManager>.Instance.announcementsVisible = true;
                mainMenuManager.StartCoroutine(mainMenuManager.announcementPopUp.ShowIfNew(new Action(delegate
                {
                    DestroyableSingleton<EOSManager>.Instance.announcementsVisible = false;
                })));
            }
            mainMenuManager.CheckAddOns();
            DestroyableSingleton<StoreMenu>.Instance.OnSaveDataChanged();
            if (!DataManager.Settings.DoesFileExist())
            {
                DataManager.Settings.ForceSave();
            }
            Logger.GlobalInstance.Info("MainMenuManager.RunStartUp finished", null);
            ControllerManager.Instance.NewScene(mainMenuManager.name, null, mainMenuManager.DefaultButtonSelected, mainMenuManager.ControllerSelectable, false);
            switch (AmongUsClient.Instance.MenuTarget)
            {
                case AmongUsClient.MainMenuTarget.OnlineMenu:
                    mainMenuManager.OpenOnlineMenu();
                    break;
                case AmongUsClient.MainMenuTarget.EnterCodeMenu:
                    mainMenuManager.OpenEnterCodeMenu(false);
                    break;
            }
            AmongUsClient.Instance.MenuTarget = AmongUsClient.MainMenuTarget.None;
            mainMenuManager.finishStartup = true;
            if (DestroyableSingleton<DisconnectPopup>.Instance.gameObject.activeSelf)
            {
                DestroyableSingleton<DisconnectPopup>.Instance.RegainUIControl();
            }
            yield return null;
            yield break;
        }
        public static bool ShipsLoaded;
    }
}
