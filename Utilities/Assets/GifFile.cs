using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Xml.Schema.FacetsChecker.FacetsCompiler;

namespace FungleAPI.Utilities.Assets
{
    /// <summary>
    /// 
    /// </summary>
    public class GifFile
    {
        public bool Loop;
        public float[] Delays;
        public Sprite[] Sprites;
        public float maxTime;
        /// <summary>
        /// 
        /// </summary>
        public Sprite GetSprite(float time)
        {
            if (!Loop && time >= maxTime)
            {
                return Sprites[Sprites.Length - 1];
            }
            float realTime = time % maxTime;
            float timer = 0;
            for (int i = 0; i < Delays.Length; i++)
            {
                timer += Delays[i];
                if (realTime < timer)
                    return Sprites[i];
            }
            return Sprites[Sprites.Length - 1];
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetGif(Sprite[] sprites, float[] delays)
        {
            foreach (float delay in delays)
            {
                maxTime += delay;
            }
            Sprites = sprites;
            Delays = delays;
        }
    }
}
