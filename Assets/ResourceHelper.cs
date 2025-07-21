using AsmResolver.PE.DotNet.ReadyToRun;
using AsmResolver.PE.Win32Resources;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using Il2CppInterop.Runtime;
using Il2CppSystem.IO;
using Rewired.UI;
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
        public static List<Sprite> LoadSprites(ModPlugin plugin, string resource, float PixelPerUnit, int tileWidth, bool dontUnload = true)
        {
            List<Sprite> sprites = new List<Sprite>();
            resource += ".png";
            System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(resource);
            if (stream == null)
                return null;

            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            stream.CopyTo(memoryStream);
            texture.LoadImage(memoryStream.ToArray());
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
        public static SpriteSheet ConvertToSpriteSheet(Sprite[] sprites, float animationSpeed)
        {
            SpriteSheet anim = new SpriteSheet();
            anim.SpriteChangeSpeed = animationSpeed;
            anim.Sprites = sprites;
            return anim;
        }
        public static Sprite LoadSprite(ModPlugin plugin, string resource, float PixelPerUnit, bool dontUnload = true)
        {
            resource = resource + ".png";
            System.IO.Stream manifestResourceStream = plugin.ModAssembly.GetManifestResourceStream(resource);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            manifestResourceStream.CopyTo(memoryStream);
            texture2D.LoadImage(memoryStream.ToArray());
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
            if (dontUnload)
            {
                sprite.DontUnload();
            }
            return sprite;
        }
        public static Transform parent;
    }
}
