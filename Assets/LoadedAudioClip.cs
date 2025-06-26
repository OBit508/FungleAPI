using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using Mono.Cecil;
using UnityEngine;

namespace FungleAPI.Assets
{
    public class LoadedAudioClip : LoadedAsset
    {
        public LoadedAudioClip(string path, ModPlugin plugin)
        {
            this.path = path + ".wav";
            using (System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(this.path))
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length); 
                this.ChannelCount = (int)array[22];
                this.Frequency = (int)array[24] | ((int)array[25] << 8);
                int num = 44;
                int num2 = (array.Length - num) / 2;
                this.SampleCount = num2;
                this.LeftChannel = new float[num2];
                for (int i = 0; i < num2; i++)
                {
                    short num3 = (short)((int)array[num] | ((int)array[num + 1] << 8));
                    this.LeftChannel[i] = (float)num3 / 32768f;
                    num += 2;
                }
            }
            Assets.Add(this);
        }
        internal float[] LeftChannel;
        internal int ChannelCount;
        internal int SampleCount;
        internal int Frequency;
        internal string path;
        internal AudioClip clip;
        public override AudioClip GetAsset()
        {
            if (clip == null)
            {
                if (parent == null)
                {
                    parent = new GameObject("All Clips").transform.DontDestroy();
                }
                AudioSource storage = new GameObject(path).AddComponent<AudioSource>();
                storage.transform.SetParent(parent);
                storage.clip = AudioClip.Create(path, SampleCount, ChannelCount, Frequency, false);
                storage.clip.SetData(LeftChannel, 0);
                clip = storage.clip;
            }
            return clip;
        }
        internal static Transform parent;
    }
}
