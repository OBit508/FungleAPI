using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Sound
{
    public class DynamicSound
    {
        public string Name;
        public AudioSource Player;
        public void Update(float dt)
        {
            if (!this.Player.isPlaying)
            {
                return;
            }
            this.volumeFunc(this.Player, dt);
        }
        public void SetTarget(AudioClip clip, DynamicSound.GetDynamicsFunction volumeFunc)
        {
            this.volumeFunc = volumeFunc;
            this.Player.clip = clip;
            this.volumeFunc(this.Player, 1f);
            this.Player.Play();
        }
        public DynamicSound.GetDynamicsFunction volumeFunc;
        public delegate void GetDynamicsFunction(AudioSource source, float dt);
    }
}
