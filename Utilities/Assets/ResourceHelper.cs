using AsmResolver.PE.DotNet.ReadyToRun;
using AsmResolver.PE.Win32Resources;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Resources;
using Il2CppSystem.Runtime.Serialization;
using Mono.Cecil;
using Rewired.UI;
using Rewired.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using xCloud;

namespace FungleAPI.Utilities.Assets
{
    /// <summary>
    /// A asset loader class
    /// </summary>
    public static class ResourceHelper
    {
        internal static Action loadAssets = new Action(FungleAssets.LoadAll);
        /// <summary>
        /// Reads an embedded text resource from the plugin assembly
        /// </summary>
        public static string ReadText(ModPlugin plugin, string resource)
        {
            using (Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// Loads an asset from an AssetBundle with optional DontUnload flag
        /// </summary>
        public static T LoadAsset<T>(this AssetBundle bundle, string name, bool dontUnload = true) where T : UnityEngine.Object
        {
            T asset = bundle.LoadAsset(name, Il2CppType.Of<T>()).SafeCast<T>();
            if (dontUnload)
            {
                asset.DontUnload();
            }
            return asset;
        }
        /// <summary>
        /// Loads an AssetBundle from an embedded resource
        /// </summary>
        public static AssetBundle LoadBundle(ModPlugin plugin, string resource)
        {
            using (Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return AssetBundle.LoadFromMemory(ms.ToArray());
                }
            }
        }
        /// <summary>
        /// Loads a WAV audio clip from an embedded resource
        /// </summary>
        public static AudioClip LoadAudio(ModPlugin plugin, string resource, string clipName, bool dontUnload = true)
        {
            resource += ".wav";
            using (Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.BaseStream.Seek(12, SeekOrigin.Begin);
                    int channels = 0;
                    int sampleRate = 0;
                    int bitsPerSample = 0;
                    byte[] audioData = null;
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                        int chunkSize = reader.ReadInt32();
                        if (chunkId == "fmt ")
                        {
                            reader.ReadInt16();
                            channels = reader.ReadInt16();
                            sampleRate = reader.ReadInt32();
                            reader.ReadInt32();
                            reader.ReadInt16();
                            bitsPerSample = reader.ReadInt16();
                        }
                        else if (chunkId == "data")
                        {
                            audioData = reader.ReadBytes(chunkSize);
                            break;
                        }
                        else
                        {
                            reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                        }
                    }
                    float[] samples = ConvertPcmToFloat(audioData, bitsPerSample);
                    AudioClip clip = AudioClip.Create(clipName, samples.Length / channels, channels, sampleRate, false);
                    if (dontUnload)
                    {
                        clip.DontUnload();
                    }
                    clip.SetData(samples, 0);
                    return clip;
                }
            }
        }
        /// <summary>
        /// Loads a GIF file from an embedded resource and converts it into a GifFile
        /// </summary>
        public static GifFile LoadGif(ModPlugin plugin, string resource, float PixelPerUnit, bool loop = true)
        {
            resource += ".gif";
            GifDecoder decoder = new GifDecoder();
            using (MemoryStream stream = new MemoryStream())
            {
                using (Stream s = plugin.ModAssembly.GetManifestResourceStream(resource))
                {
                    s.CopyTo(stream);
                    decoder.LoadGif(stream.ToArray());
                    GifFile gif = new GifFile();
                    gif.Loop = loop;
                    List<Sprite> sprites = new List<Sprite>();
                    for (int i = 0; i < decoder.Frames.Count; i++)
                    {
                        Texture2D texture = decoder.Frames[i];
                        sprites.Add(Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelPerUnit).DontUnload());
                    }
                    gif.SetGif(sprites.ToArray(), decoder.FrameDelays.ToArray());
                    return gif;
                }
            }
        }
        /// <summary>
        /// Loads a PNG sprite from an embedded resource
        /// </summary>
        public static Sprite LoadSprite(ModPlugin plugin, string resource, float PixelPerUnit, bool dontUnload = true)
        {
            resource = resource + ".png";
            using (Stream manifestResourceStream = plugin.ModAssembly.GetManifestResourceStream(resource))
            {
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    manifestResourceStream.CopyTo(memoryStream);
                    texture2D.LoadImage(memoryStream.ToArray(), false);
                    texture2D.Apply();
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
                    if (dontUnload)
                    {
                        sprite.DontUnload();
                    }
                    return sprite;
                }
            }
        }
        /// <summary>
        /// Converts an array of sprites into a GifFile with uniform delay
        /// </summary>
        public static GifFile ToGif(Sprite[] sprites, float delay, bool loop = true)
        {
            GifFile animation = new GifFile();
            animation.Sprites = sprites;
            animation.Delays = new float[sprites.Count()];
            for (int i = 0; i < animation.Delays.Count(); i++)
            {
                animation.Delays[i] = delay;
            }
            animation.SetGif(animation.Sprites, animation.Delays);
            animation.Loop = loop;
            return animation;
        }
        private static float[] ConvertPcmToFloat(byte[] data, int bitsPerSample)
        {
            int sampleSize = bitsPerSample / 8;
            int sampleCount = data.Length / sampleSize;
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                int index = i * sampleSize;
                samples[i] = bitsPerSample switch
                {
                    8 => (data[index] - 128) / 128f,
                    16 => BitConverter.ToInt16(data, index) / 32768f,
                    24 => (((data[index + 2] << 24) | (data[index + 1] << 16) | (data[index] << 8)) >> 8) / 8388608f,
                    32 => BitConverter.ToInt32(data, index) / 2147483648f,
                    _ => 0
                };
            }
            return samples;
        }
    }
}
