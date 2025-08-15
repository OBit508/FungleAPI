using FungleAPI.Assets;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class PlayerAnimator : PlayerComponent
    {
        public PlayerControl Player => GetComponent<PlayerControl>();
        public GifFile RunAnim;
        public GifFile Idle;
        public GifFile CustomAnimation;
        public GifAnimator Animator;
        internal bool play;
        public void Start()
        {
            Animator = Player.gameObject.AddComponent<GifAnimator>();
            Animator.spriteRenderer = Player.cosmetics.currentBodySprite.BodySprite;
        }
        public void Play()
        {
            play = true;
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void Stop()
        {
            play = false;
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void PlayAnimation(Assets.GifFile anim)
        {
            play = false;
            CustomAnimation = anim;
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void Update()
        {
            GifFile anim = null;
            Action EndAnim = null;
            if (CustomAnimation != null)
            {
                anim = CustomAnimation;
                EndAnim = new Action(delegate
                {
                    CustomAnimation = null;
                    Play();
                });
            }
            else if (play)
            {
                anim = Idle;
                if (Player.MyPhysics.Animations.IsPlayingRunAnimation())
                {
                    anim = RunAnim;
                }
            }
            if (Animator.spriteRenderer != Player.cosmetics.currentBodySprite.BodySprite)
            {
                Animator.spriteRenderer = Player.cosmetics.currentBodySprite.BodySprite;
                Animator.SetAnimation(anim, false);
                Animator.EndAnim = EndAnim;
            }
        }
    }
}
