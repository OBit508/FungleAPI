using FungleAPI.Attributes;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.Controller;

namespace FungleAPI.Components
{
    /// <summary>
    /// A component that animates SpriteRenderers using a given GifFile
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class GifAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public GifFile Gif;
        public float speed = 1;
        public float timer;
        public bool stoped;
        /// <summary>
        /// Play the animation from where it left off
        /// </summary>
        public void Play(float speed = 1)
        {
            this.speed = speed;
            stoped = false;
        }
        /// <summary>
        /// Pause the animation
        /// </summary>
        public void Pause()
        {
            speed = 0;
        }
        /// <summary>
        /// Pause and reset the animation
        /// </summary>
        public void Stop()
        {
            stoped = true;
            timer = 0;
        }
        public void Update()
        {
            if (Gif != null && !stoped)
            {
                timer += Time.deltaTime * speed;
                spriteRenderer.sprite = Gif.GetSprite(timer);
            }
        }
    }
}
