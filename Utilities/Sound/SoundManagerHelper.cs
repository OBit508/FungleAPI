using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace FungleAPI.Utilities.Sound
{
    [HarmonyPatch(typeof(SoundManager))]
    public static class SoundManagerHelper
    {
        private static List<DynamicSound> soundPlayers = new List<DynamicSound>();
        [HarmonyPatch("HasNamedSound")]
        [HarmonyPostfix]
        private static void HasNamedSoundPostfix([HarmonyArgument(0)] string name, ref bool __result)
        {
            if (!__result)
            {
                using (List<DynamicSound>.Enumerator enumerator = soundPlayers.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Name.Equals(name))
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
        [HarmonyPatch("StopNamedSound")]
        [HarmonyPostfix]
        private static void StopNamedSoundPostfix([HarmonyArgument(0)] string name)
        {
            for (int i = soundPlayers.Count - 1; i >= 0; i--)
            {
                DynamicSound soundPlayer = soundPlayers[i];
                if (soundPlayer.Name.Equals(name))
                {
                    global::UnityEngine.Object.Destroy(soundPlayer.Player);
                    soundPlayers.RemoveAt(i);
                    return;
                }
            }
        }
        [HarmonyPatch("StopSound")]
        [HarmonyPostfix]
        private static void StopSoundPostfix([HarmonyArgument(0)] AudioClip clip)
        {
            for (int i = 0; i < soundPlayers.Count; i++)
            {
                DynamicSound soundPlayer = soundPlayers[i];
                if (soundPlayer.Player.clip == clip)
                {
                    global::UnityEngine.Object.Destroy(soundPlayer.Player);
                    soundPlayers.RemoveAt(i);
                    return;
                }
            }
        }
        [HarmonyPatch("StopAllSound")]
        [HarmonyPostfix]
        private static void StopAllSoundPostfix()
        {
            for (int i = 0; i < soundPlayers.Count; i++)
            {
                global::UnityEngine.Object.Destroy(soundPlayers[i].Player);
            }
            soundPlayers.Clear();
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix()
        {
            for (int i = 0; i < soundPlayers.Count; i++)
            {
                soundPlayers[i].Update(Time.deltaTime);
            }
        }
        public static AudioSource PlayDynamicSound(string name, AudioClip clip, bool loop, DynamicSound.GetDynamicsFunction volumeFunc, AudioMixerGroup channel)
        {
            DynamicSound dynamicSound = null;
            for (int i = 0; i < soundPlayers.Count; i++)
            {
                DynamicSound soundPlayer = soundPlayers[i];
                if (soundPlayer.Name == name)
                {
                    dynamicSound = soundPlayer;
                    break;
                }
            }
            if (dynamicSound == null)
            {
                dynamicSound = new DynamicSound();
                dynamicSound.Name = name;
                dynamicSound.Player = SoundManager.Instance.gameObject.AddComponent<AudioSource>();
                dynamicSound.Player.outputAudioMixerGroup = channel;
                dynamicSound.Player.playOnAwake = false;
                soundPlayers.Add(dynamicSound);
            }
            dynamicSound.Player.loop = loop;
            dynamicSound.SetTarget(clip, volumeFunc);
            return dynamicSound.Player;
        }
    }
}
