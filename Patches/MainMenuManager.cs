using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Utilities.Assets;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), "Start")]
    internal static class MainMenuManagerPatch
    {
        public static bool Prefix(MainMenuManager __instance)
        {
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
            FungleAssets.CreditsPrefab.Instantiate(__instance.transform.GetChild(5).GetChild(1).GetChild(3));
            return false;
        }
        public static System.Collections.IEnumerator RunStartUp(MainMenuManager mainMenuManager)
        {
            mainMenuManager.finishStartup = false;
            Logger.GlobalInstance.Info("MainMenuManager.RunStartUp beginning", null);
            Constants.SetMainThread();
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
            yield return Utilities.Prefabs.PrefabUtils.CoLoadShipPrefabs();
            mainMenuManager.finishStartup = true;
            if (DestroyableSingleton<DisconnectPopup>.Instance.gameObject.activeSelf)
            {
                DestroyableSingleton<DisconnectPopup>.Instance.RegainUIControl();
            }
            yield return null;
            yield break;
        }
    }
}
