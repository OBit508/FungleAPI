using AsmResolver.PE.DotNet.ReadyToRun;
using AsmResolver.PE.Win32Resources;
using FungleAPI.MonoBehaviours;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using Il2CppSystem.Runtime.Serialization;
using Mono.Cecil;
using Rewired.UI;
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

namespace FungleAPI.Assets
{
    public static class ResourceHelper
    {
        public static Sprite EmptySprite = LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.empty", 100);
        public static string ReadText(ModPlugin plugin, string resource)
        {
            return new System.IO.StreamReader(plugin.ModAssembly.GetManifestResourceStream(resource)).ReadToEnd();
        }
        public static T LoadAsset<T>(AssetBundle bundle, string name, bool dontUnload = true) where T : UnityEngine.Object
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
            resource = resource + ".wav";
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            WAV wav = new WAV(array);
            AudioClip audioClip = AudioClip.Create(resource, wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            if (dontUnload)
            {
                audioClip.DontUnload();
            }
            return audioClip;
        }
        public static List<Sprite> LoadSpriteSheet(ModPlugin plugin, string resource, float PixelPerUnit, int tileWidth, bool dontUnload = true)
        {
            List<Sprite> sprites = new List<Sprite>();
            resource += ".png";
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            if (stream == null)
                return null;

            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            stream.CopyTo(memoryStream);
            LoadImage(texture, memoryStream.ToArray(), true);
            int tileHeight = texture.height;
            int totalTiles = texture.width / tileWidth;
            for (int i = 0; i < totalTiles; i++)
            {
                Rect rect = new Rect(i * tileWidth, 0, tileWidth, tileHeight);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(texture, rect, pivot, PixelPerUnit);
                if (dontUnload)
                {
                    sprite.DontUnload();
                }
                sprites.Add(sprite);
            }
            return sprites;
        }
        public static GifFile ToGif(Sprite[] sprites, float delay)
        {
            GifFile animation = ScriptableObject.CreateInstance<GifFile>().DontUnload();
            animation.Frames = new Dictionary<ChangeableValue<float>, Sprite>();
            foreach (Sprite sprite in sprites)
            {
                animation.Frames.Add(new ChangeableValue<float>(delay), sprite);
            }
            return animation;
        }
        public static GifFile LoadGif(ModPlugin plugin, string resource, float PixelPerUnit, bool loop = true)
        {
            resource += ".gif";
            GifDecoder decoder = new GifDecoder();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            plugin.ModAssembly.GetManifestResourceStream(resource).CopyTo(stream);
            decoder.LoadGif(stream.ToArray());
            GifFile gif = ScriptableObject.CreateInstance<GifFile>().DontUnload();
            gif.Loop = loop;
            for (int i = 0; i < decoder.Frames.Count; i++)
            {
                Texture2D texture = decoder.Frames[i];
                gif.Frames.Add(new ChangeableValue<float>(decoder.FrameDelays[i]), Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelPerUnit).DontUnload());
            }
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
        private static bool LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            if (iCall_LoadImage == null)
                iCall_LoadImage = IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

            var il2cppArray = (Il2CppStructArray<byte>)data;

            return iCall_LoadImage.Invoke(tex.Pointer, il2cppArray.Pointer, markNonReadable);
        }
        internal delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
        internal static d_LoadImage iCall_LoadImage;
    }
}
