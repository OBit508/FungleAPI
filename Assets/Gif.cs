using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Xml.Schema.FacetsChecker.FacetsCompiler;

namespace FungleAPI.Assets
{
    public class Gif
    {
        public bool Loop;
        public IEnumerable<float> Delays;
        public IEnumerable<Texture2D> Frames;
        public float maxTime;
        /// <summary>
        /// Returns the frame corresponding to the given time
        /// </summary>
        public Texture2D GetFrame(float time)
        {
            if (!Loop && time >= maxTime)
            {
                return Frames.Last();
            }
            float realTime = time % maxTime;
            float timer = 0;
            for (int i = 0; i < Delays.Count(); i++)
            {
                timer += Delays.ElementAt(i);
                if (realTime < timer)
                {
                    return Frames.ElementAt(i);
                }
            }
            return Frames.Last();
        }
        /// <summary>
        /// Sets the sprite frames and their respective delays
        /// </summary>
        public void SetGif(IEnumerable<Texture2D> frames, IEnumerable<float> delays)
        {
            foreach (float delay in delays)
            {
                maxTime += delay;
            }
            Frames = frames;
            Delays = delays;
        }
    }
}