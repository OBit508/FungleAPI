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
        public SpriteRenderer renderer;
        public SpriteSheet animation;
        public bool canPlay;
        public bool loop;
        public int currentSprite;
        public float timer;
        public void PlayAnimation(bool loop)
        {
            this.loop = loop;
            canPlay = true;
        }
        public void StopAnimation()
        {
            canPlay = false;
        }
        public void SetAnimation(SpriteSheet newAnim)
        {
            if (animation != newAnim)
            {
                animation = newAnim;
                currentSprite = 0;
            }
        }
        public void Update()
        {
            if (animation != null && canPlay)
            {
                renderer.sprite = animation.Sprites[currentSprite];
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    timer = animation.SpriteChangeSpeed;
                    if (currentSprite + 1 >= animation.Sprites.Count())
                    {
                        currentSprite = 0;
                        if (!loop)
                        {
                            canPlay = false;
                        }
                    }
                    else
                    {
                        currentSprite++;
                    }
                }
            }
        }
        public static SpriteAnimator AddCustomAnimator(SpriteRenderer renderer)
        {
            SpriteAnimator animator = renderer.gameObject.AddComponent<SpriteAnimator>();
            animator.renderer = renderer;
            return animator;
        }
    }
}
