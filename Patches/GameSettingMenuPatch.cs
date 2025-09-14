using FungleAPI.Role.Patches;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(GameSettingMenu), "Start")]
    internal static class GameSettingMenuPatch
    {
        public static int currentIndex;
        public static ModPlugin currentPlugin;
        public static void Postfix(GameSettingMenu __instance)
        {
            currentIndex = 0;
            currentPlugin = FungleAPIPlugin.Plugin;
            PassiveButton SwitchButton = GameObject.Instantiate(__instance.ControllerSelectable[0].SafeCast<PassiveButton>(), __instance.ControllerSelectable[0].transform.parent);
            SwitchButton.transform.localPosition = new Vector3(-2.96f, 1.57f, -2);
            SwitchButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            SwitchButton.OnClick.AddListener(new Action(delegate
            {
                if (currentIndex + 1 >= ModPlugin.AllPlugins.Count)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex++;
                }
                currentPlugin = ModPlugin.AllPlugins[currentIndex];
                SwitchButton.buttonText.text = currentPlugin.ModName;
                RolesSettingMenuPatch.chanceTabPlugin = null;
                __instance.GameSettingsTab.Initialize();
            }));
            SwitchButton.buttonText.GetComponent<TextTranslatorTMP>().enabled = false;
            SwitchButton.buttonText.text = currentPlugin.ModName;
            __instance.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
