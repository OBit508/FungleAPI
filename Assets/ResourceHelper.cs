using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppInterop.Runtime;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using Il2CppSystem.IO;
using System.Runtime.CompilerServices;
using FungleAPI.MonoBehaviours;

namespace FungleAPI.Assets
{
    public static class ResourceHelper
    {
        public static AudioClip LoadAudio(string resource, string clipName)
        {
            resource = resource + ".wav";
            using (System.IO.Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resource))
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                WAV wav = new WAV(array);
                AudioClip audioClip = AudioClip.Create(resource, wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
                audioClip.SetData(wav.LeftChannel, 0);
                return audioClip;
            }
        }
        public static List<Sprite> LoadSprites(string resource, float PixelPerUnit, int tileWidth)
        {
            List<Sprite> sprites = new List<Sprite>();
            try
            {
                resource += ".png";
                using (System.IO.Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resource))
                {
                    if (stream == null)
                        return null;

                    Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        texture.LoadImage(memoryStream.ToArray());
                    }
                    int tileHeight = texture.height;
                    int totalTiles = texture.width / tileWidth;
                    for (int i = 0; i < totalTiles; i++)
                    {
                        Rect rect = new Rect(i * tileWidth, 0, tileWidth, tileHeight);
                        Vector2 pivot = new Vector2(0.5f, 0.5f);
                        Sprite sprite = Sprite.Create(texture, rect, pivot, PixelPerUnit);
                        sprites.Add(sprite);
                    }
                }
            }
            catch
            {
            };
            return sprites;
        }
        public static SpriteSheet ConvertToSpriteSheet(Sprite[] sprites, float animationSpeed)
        {
            SpriteSheet anim = new SpriteSheet();
            anim.SpriteChangeSpeed = animationSpeed;
            anim.Sprites = sprites;
            return anim;
        }
        public static Sprite LoadSprite(string resource, float PixelPerUnit)
        {
            try
            {
                resource = resource + ".png";
                System.IO.Stream manifestResourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resource);
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                Sprite sprite2;
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    manifestResourceStream.CopyTo(memoryStream);
                    texture2D.LoadImage(memoryStream.ToArray());
                    Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), PixelPerUnit);
                    sprite2 = sprite;
                }
                return sprite2;
            }
            catch
            {
            }
            return null;
        }
        public static Transform parent;
    }
}
