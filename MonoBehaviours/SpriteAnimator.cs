using FungleAPI.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class SpriteAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public SpriteAnimation anim;
        public bool canPlay;
        public int currentSprite;
        public float timer;
        public Action EndAnim;
        public Action StartAnim;
        public void PlayAnimation(bool reset = true, bool playStartEvent = true)
        {
            if (playStartEvent)
            {
                StartAnim?.Invoke();
            }
            if (reset)
            {
                currentSprite = 0;
                timer = 0;
            }
            canPlay = true;
        }
        public void StopAnimation(bool playEndEvent = true)
        {
            if (playEndEvent)
            {
                EndAnim?.Invoke();
            }
            canPlay = false;
        }
        public void SetAnimation(SpriteAnimation animation, bool playStartEvent = true, bool setAnimEvents = true)
        {
            if (anim != animation)
            {
                anim = animation;
                currentSprite = 0;
                timer = 0;
                if (setAnimEvents)
                {
                    StartAnim = animation.StartAnim;
                    EndAnim = animation.EndAnim;
                }
                if (canPlay && playStartEvent)
                {
                    StartAnim?.Invoke();
                }
            }
        }
        public void Update()
        {
            if (anim != null && canPlay && spriteRenderer != null)
            {
                Sprite sprite = anim.Frames.Keys.ToArray()[currentSprite];
                spriteRenderer.sprite = sprite;
                timer += Time.deltaTime;
                if (timer >= anim.Frames[sprite].Value)
                {
                    if (currentSprite + 1 >= anim.Frames.Count())
                    {
                        currentSprite = 0;
                        if (!anim.Loop)
                        {
                            canPlay = false;
                            EndAnim?.Invoke();
                        }
                    }
                    else
                    {
                        currentSprite++;
                    }
                    timer = anim.Frames.Values.ToArray()[currentSprite].Value;
                }
            }
        }
    }
}
