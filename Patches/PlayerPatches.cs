using AmongUs.GameOptions;
using AsmResolver.PE.DotNet.ReadyToRun;
using Epic.OnlineServices.Presence;
using FungleAPI.Assets;
using FungleAPI.MonoBehaviours;
using FungleAPI.Role;
using FungleAPI.Role.Teams;
using FungleAPI.Roles;
using FungleAPI.Rpc;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Net;
using Rewired;
using Rewired.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    public static class PlayerPatches
    {
        internal static List<Il2CppSystem.Type> AllPlayerComponents = new List<Il2CppSystem.Type>();
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void OnStart(PlayerControl __instance)
        {
            foreach (Il2CppSystem.Type type in AllPlayerComponents)
            {
                __instance.gameObject.AddComponent(type);
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch("RpcMurderPlayer")]
        public static bool PlayerControlMurderPrefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target, [HarmonyArgument(1)] bool didSucceed)
        {
            RpcCustomMurderPlayer(__instance, target, MurderResultFlags.Succeeded);
            return false;
        }
        [HarmonyPatch("ToggleHighlight")]
        [HarmonyPrefix]
        public static bool OnToggleHighlight(PlayerControl __instance, [HarmonyArgument(0)] bool active)
        {
            if (__instance.Data.Role.CustomRole() != null && active)
            {
                __instance.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                __instance.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", __instance.Data.Role.CustomRole().Configuration.OutlineColor);
                return false;
            }
            return true;
        }
        public static void RpcCustomMurderPlayer(this PlayerControl killer, PlayerControl target, MurderResultFlags resultFlags, bool resetKillTimer = true, bool createDeadBody = true, bool teleportMurderer = true, bool showKillAnim = true, bool playKillSound = true)
        {
            CustomRpcManager.GetInstance<RpcCustomMurder>().Send((killer, target, resultFlags, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound), killer.NetId);
        }
        public static T GetPlayerComponent<T>(this PlayerControl player) where T : PlayerComponent
        {
            return player.GetComponent<T>();
        }
        public static PlayerControl GetClosest(this PlayerControl target)
        {
            PlayerControl closest = null;
            float dis = target.Data.Role.GetAbilityDistance();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                Vector3 center = target.Collider.bounds.center;
                Vector3 position = player.transform.position;
                float num = Vector2.Distance(center, position);
                if (player != target && !player.Data.IsDead && !PhysicsHelpers.AnythingBetween(target.Collider, center, position, Constants.ShipOnlyMask, false) && num < dis)
                {
                    closest = player;
                    dis = num;
                }
            }
            return closest;
        }
    }
    public class PlayerHelper : PlayerComponent
    {
        internal static Dictionary<Animator, PlayerHelper> anims = new Dictionary<Animator, PlayerHelper>();
        internal static Dictionary<Animator, PlayerHelper> Animators
        {
            get
            {
                Animator[] keys = new Animator[anims.Count];
                anims.Keys.CopyTo(keys, 0);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].IsNullOrDestroyed())
                    {
                        anims.Remove(keys[i]);
                    }
                }
                return anims;
            }
        }
        public RoleBehaviour OldRole = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
        public GifFile IdleAnim;
        public GifFile RunAnim;
        public GifFile GhostIdleAnim;
        public GifFile EnterVentAnim;
        public GifFile ExitVentAnim;
        public GifFile SpawnAnim;
        public GifFile SpawnGlowAnim;
        public GifFile ClimbUpAnim;
        public GifFile ClimbDownAnim;
        public GifFile GhostGuardianAngelAnim;
        public GifFile Custom;
        public GifFile Current;
        public bool CanPlay;
        internal Action EndCustom;
        internal float timer;
        internal int currentSprite;
        public PlayerControl Player => GetComponent<PlayerControl>();
        public PlayerAnimationGroup Group => Player.MyPhysics.Animations.group;
        public void Reset()
        {
            currentSprite = 0;
            timer = 0;
        }
        public void Play()
        {
            CanPlay = true;
            Reset();
            Player.MyPhysics.ResetAnimState();
        }
        public void Stop()
        {
            CanPlay = false;
            Player.MyPhysics.ResetAnimState();
        }
        public void PlayCustom(GifFile custom, Action onEnd = null)
        {
            Custom = custom;
            EndCustom = onEnd;
            CanPlay = true;
            Reset();
            Player.MyPhysics.ResetAnimState();
        }
        public void Update()
        {
            Animator animator = Player.MyPhysics.Animations.Animator.m_animator;
            if (!Animators.ContainsKey(animator))
            {
                Animators.Add(animator, this);
            }
            if (CanPlay)
            {
                TryGetCurrentAnimation();
                if (Current != null)
                {
                    if (!Current.Sprites.Contains(Group.SpriteAnimator.m_nodes.m_spriteRenderer.sprite))
                    {
                        Player.MyPhysics.ResetAnimState();
                    }
                    timer += Time.deltaTime;
                    if (timer >= Current.Delays[currentSprite])
                    {
                        if (currentSprite + 1 >= Current.Sprites.Count())
                        {
                            currentSprite = 0;
                            if (Custom == Current && EndCustom != null)
                            {
                                Stop();
                                Custom = null;
                                EndCustom?.Invoke();
                            }
                        }
                        else
                        {
                            currentSprite++;
                        }
                        timer = 0;
                    }
                }
            }
        }
        public void TryGetCurrentAnimation()
        {
            GifFile Current = Custom;
            if (Current == null)
            {
                if (Group.SpriteAnimator.m_currAnim == Group.RunAnim)
                {
                    Current = RunAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.IdleAnim)
                {
                    Current = IdleAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.GhostIdleAnim)
                {
                    Current = GhostIdleAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.EnterVentAnim)
                {
                    Current = EnterVentAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.ExitVentAnim)
                {
                    Current = ExitVentAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.SpawnAnim)
                {
                    Current = SpawnAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.SpawnGlowAnim)
                {
                    Current = SpawnGlowAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.ClimbUpAnim)
                {
                    Current = ClimbUpAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.ClimbDownAnim)
                {
                    Current = ClimbDownAnim;
                }
                else if (Group.SpriteAnimator.m_currAnim == Group.GhostGuardianAngelAnim)
                {
                    Current = GhostGuardianAngelAnim;
                }
            }
            if (Current != this.Current)
            {
                this.Current = Current;
                Reset();
            }
        }
    }
}
