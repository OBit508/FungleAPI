using AsmResolver.PE.DotNet.ReadyToRun;
using FungleAPI.Extensions;
using HarmonyLib;
using Il2CppSystem.Threading.Tasks;
using PowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FungleAPI.GameModes.Patches
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    internal static class SpawnInMinigamePatch
    {
        public static bool Prefix(SpawnInMinigame __instance, PlayerTask task)
        {
            Minigame.Instance = __instance;
            __instance.MyTask = task;
            __instance.MyNormTask = task as NormalPlayerTask;
            __instance.timeOpened = Time.realtimeSinceStartup;
            if (PlayerControl.LocalPlayer)
            {
                if (MapBehaviour.Instance)
                {
                    MapBehaviour.Instance.Close();
                }
                PlayerControl.LocalPlayer.MyPhysics.SetNormalizedVelocity(Vector2.zero);
            }
            __instance.logger.Info("Opening minigame " + __instance.GetType().Name, null);
            __instance.StartCoroutine(__instance.CoAnimateOpen());
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.MinigameOpened(PlayerControl.LocalPlayer.Data, __instance.TaskType);

            SpawnInMinigame.SpawnLocation[] array = __instance.Locations.ToArray<SpawnInMinigame.SpawnLocation>();
            for (int i = 0; i < array.Count() - 1; i++)
            {
                SpawnInMinigame.SpawnLocation t = array[i];
                int num = global::UnityEngine.Random.Range(i, array.Count());
                array[i] = array[num];
                array[num] = t;
            }
            array = (from s in array.Take<SpawnInMinigame.SpawnLocation>(__instance.LocationButtons.Length)
                     orderby s.Location.x, s.Location.y descending
                     select s).ToArray<SpawnInMinigame.SpawnLocation>();
            for (int i = 0; i < __instance.LocationButtons.Length; i++)
            {
                PassiveButton passiveButton = __instance.LocationButtons[i];
                SpawnInMinigame.SpawnLocation pt = array[i];
                passiveButton.OnClick.AddListener(new Action(delegate
                {
                    __instance.SpawnAt(pt);
                }));
                passiveButton.GetComponent<SpriteAnim>().Stop();
                passiveButton.GetComponent<SpriteRenderer>().sprite = pt.Image;
                passiveButton.GetComponentInChildren<TextMeshPro>().text = DestroyableSingleton<TranslationController>.Instance.GetString(pt.Name);
                ButtonAnimRolloverHandler component = passiveButton.GetComponent<ButtonAnimRolloverHandler>();
                component.StaticOutImage = pt.Image;
                component.RolloverAnim = pt.Rollover;
                component.HoverSound = (pt.RolloverSfx ? pt.RolloverSfx : __instance.DefaultRolloverSound);
            }
            if (GameManager.Instance != null && !GameManager.Instance.IsHideAndSeek())
            {
                foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
                {
                    if (!(networkedPlayerInfo == null) && !(networkedPlayerInfo.Object == null) && !networkedPlayerInfo.Disconnected && !networkedPlayerInfo.Object.isDummy)
                    {
                        networkedPlayerInfo.Object.NetTransform.transform.position = new Vector2(-25f, 40f);
                        networkedPlayerInfo.Object.NetTransform.Halt();
                    }
                }
            }
            __instance.StartCoroutine(__instance.RunTimer());
            ControllerManager.Instance.OpenOverlayMenu(__instance.name, null, __instance.DefaultButtonSelected, __instance.ControllerSelectable, false);
            PlayerControl.HideCursorTemporarily();
            ConsoleJoystick.SetMode_Menu();
            return false;
        }
    }
}
