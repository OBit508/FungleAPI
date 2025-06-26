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
    public class PlayerAnimator : MonoBehaviour
    {
        public PlayerControl Player;
        public SpriteAnimator Animator;
        public SpriteSheet RunAnim;
        public SpriteSheet Idle;
        public bool PlayingAnimation;
        public bool PlayCustomAnimation;
        public void Play()
        {
            PlayingAnimation = true;
            Animator.PlayAnimation(true);
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void Stop()
        {
            PlayingAnimation = false;
            Animator.StopAnimation();
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void PlayAnimation(SpriteSheet anim)
        {
            PlayCustomAnimation = true;
            Animator.SetAnimation(anim);
            Animator.PlayAnimation(false);
            Player.MyPhysics.Animations.PlayIdleAnimation();
        }
        public void Update()
        {
            if (Animator.canPlay && !PlayCustomAnimation)
            {
                SpriteSheet anim = Idle;
                if (Player.MyPhysics.Animations.IsPlayingRunAnimation())
                {
                    anim = RunAnim;
                }
                if (Animator.animation != anim && anim != null)
                {
                    Animator.SetAnimation(anim);
                }
            }
            else if (!Animator.canPlay && PlayCustomAnimation)
            {
                PlayCustomAnimation = false;
                if (PlayingAnimation)
                {
                    Play();
                }
            }
        }
    }
}
