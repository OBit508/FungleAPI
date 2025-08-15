using FungleAPI.Assets;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class GifAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public GifFile anim;
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
        public void SetAnimation(GifFile animation, bool playStartEvent = true)
        {
            if (anim != animation)
            {
                anim = animation;
                currentSprite = 0;
                timer = 0;
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
                ChangeableValue<float> duration = anim.Frames.Keys.ToArray()[currentSprite];
                spriteRenderer.sprite = anim.Frames[duration];
                timer += Time.deltaTime;
                if (timer >= duration.Value)
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
                    timer = 0;
                }
            }
        }
    }
}
