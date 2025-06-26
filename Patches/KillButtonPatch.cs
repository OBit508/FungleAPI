using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.MonoBehaviours;
using FungleAPI.Roles;
using HarmonyLib;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(KillButton), "Start")]
    internal class KillButtonPatch
    {
        public static float timer;
        public static PlayerControl target;
        public static void Postfix(KillButton __instance)
        {
            __instance.gameObject.AddComponent<Updater>().onUpdate = new Action(delegate
            {
                target = null;
                if (!PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Data.Role.CanUseKillButton)
                {
                    PlayerControl p = null;
                    float d = PlayerControl.LocalPlayer.Data.Role.GetAbilityDistance();
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        Vector3 center = PlayerControl.LocalPlayer.Collider.bounds.center;
                        Vector3 position = player.transform.position;
                        float num = Vector2.Distance(center, position);
                        bool flag = player.Data.Role.GetTeam() != PlayerControl.LocalPlayer.Data.Role.GetTeam();
                        if (!flag && PlayerControl.LocalPlayer.Data.Role.GetTeam().FriendlyFire)
                        {
                            flag = true;
                        }
                        if (player != PlayerControl.LocalPlayer && !player.Data.IsDead && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, center, position, Constants.ShipOnlyMask, false) && num < d && flag)
                        {
                            p = player;
                            d = num;
                        }
                    }
                    target = p;
                    if (timer >= 0)
                    {
                        timer -= Time.deltaTime;
                        __instance.SetCoolDown(timer, GameOptionsManager.Instance.currentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown));
                    }
                }
                __instance.SetTarget(target);
            });
            timer = GameOptionsManager.Instance.currentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown);
            __instance.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            __instance.GetComponent<PassiveButton>().OnClick.AddListener(new Action(delegate
            {
                if (timer <= 0 && target != null)
                {
                    timer = GameOptionsManager.Instance.currentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown);
                    PlayerControl.LocalPlayer.RpcMurderPlayer(target, true);
                }
            }));
        }
    }
}
