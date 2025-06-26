using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.MonoBehaviours;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(TaskAdderGame), "Begin")]
    internal class TaskAdderGamePatch
    {
        public static void Prefix(TaskAdderGame __instance)
        {
            __instance.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
            {
                foreach (Transform item in __instance.ActiveItems)
                {
                    if (item.GetComponent<TaskAddButton>() != null && item.GetComponent<TaskAddButton>().role != null)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            });
        }
    }
}
