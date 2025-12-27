using AsmResolver.PE.DotNet.ReadyToRun;
using AsmResolver.PE.Win32Resources;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
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
    public static class ResourceHelper
    {
        public static string ReadText(ModPlugin plugin, string resource)
        {
            return new System.IO.StreamReader(plugin.ModAssembly.GetManifestResourceStream(resource)).ReadToEnd();
        }
        public static T LoadAsset<T>(this AssetBundle bundle, string name, bool dontUnload = true) where T : UnityEngine.Object
        {
            T asset = bundle.LoadAsset(name, Il2CppType.Of<T>()).SafeCast<T>();
            if (dontUnload)
            {
                asset.DontUnload();
            }
            return asset;
        }
        public static AssetBundle LoadBundle(ModPlugin plugin, string resource)
        {
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            stream.CopyTo(ms);
            return AssetBundle.LoadFromMemory(ms.ToArray());
        }
        public static AudioClip LoadAudio(ModPlugin plugin, string resource, string clipName, bool dontUnload = true)
        {
            resource += ".wav";
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
            reader.BaseStream.Seek(12, System.IO.SeekOrigin.Begin);
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
                    reader.BaseStream.Seek(chunkSize, System.IO.SeekOrigin.Current);
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
        public static List<Sprite> LoadSpriteSheet(ModPlugin plugin, string resource, float pixelsPerUnit, int columnsY, int columnsX, bool skipEmpty = true, bool dontUnload = true)
        {
            List<Sprite> sprites = new List<Sprite>();
            resource += ".png";
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            stream.CopyTo(memoryStream);
            LoadImage(texture, memoryStream.ToArray(), false);
            float tileWidth = texture.width / columnsX;
            float tileHeight = texture.height / columnsY;
            for (int y = 0; y < columnsY; y++)
            {
                for (int x = 0; x < columnsX; x++)
                {
                    float rectY = texture.height - ((y + 1) * tileHeight);
                    Rect rect = new Rect(x * tileWidth, rectY, tileWidth, tileHeight);
                    UnityEngine.Color[] pixels = texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                    if (skipEmpty && pixels.All(p => p.a == 0f))
                    {
                        continue;
                    }
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    Sprite sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
                    if (dontUnload)
                    {
                        sprite.DontUnload();
                    }
                    sprites.Add(sprite);
                }
            }
            return sprites;
        }
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
        public static GifFile LoadGif(ModPlugin plugin, string resource, float PixelPerUnit, bool loop = true)
        {
            resource += ".gif";
            GifDecoder decoder = new GifDecoder();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            plugin.ModAssembly.GetManifestResourceStream(resource).CopyTo(stream);
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
        public static Sprite LoadSprite(ModPlugin plugin, string resource, float PixelPerUnit, bool dontUnload = true)
        {
            resource = resource + ".png";
            System.IO.Stream manifestResourceStream = plugin.ModAssembly.GetManifestResourceStream(resource);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            manifestResourceStream.CopyTo(memoryStream);
            LoadImage(texture2D, memoryStream.ToArray(), true);
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            if (dontUnload)
            {
                sprite.DontUnload();
            }
            return sprite;
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
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
            {
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");
            }
            Il2CppStructArray<byte> il2cppArray = (Il2CppStructArray<byte>)data;
            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
    }
}
