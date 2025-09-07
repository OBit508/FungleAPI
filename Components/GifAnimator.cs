using FungleAPI.Utilities;
using FungleAPI.Attributes;
using FungleAPI.Utilities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class GifAnimator : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public GifFile Gif;
        public float speed = 1;
        public float timer;
        public bool stoped;
        public void Play(float speed = 1)
        {
            this.speed = speed;
        }
        public void Pause()
        {
            speed = 0;
        }
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
