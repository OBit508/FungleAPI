using FungleAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class PlayerHelper : PlayerComponent
    {
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
        internal SpriteRenderer Renderer;
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
        public void Start()
        {
            if (Renderer == null)
            {
                Renderer = new GameObject("CustomAnimation Renderer").AddComponent<SpriteRenderer>();
                Renderer.material = Player.cosmetics.currentBodySprite.BodySprite.material;
                Renderer.transform.SetParent(Player.cosmetics.currentBodySprite.BodySprite.transform.parent);
            }
        }
        public void Update()
        {
            bool couldPlay = CanPlay;
            if (couldPlay)
            {
                TryGetCurrentAnimation();
                couldPlay = Current == null;
            }
            Group.SpriteAnimator.m_nodes.m_spriteRenderer.enabled = !couldPlay;
            Renderer.enabled = couldPlay;
            if (CanPlay)
            {
                TryGetCurrentAnimation();
                if (Current != null)
                {
                    Renderer.sprite = Current.Sprites[currentSprite];
                    if (!Current.Sprites.Contains(Renderer.sprite))
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
