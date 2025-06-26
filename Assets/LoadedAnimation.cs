using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Assets
{
    public class LoadedAnimation : LoadedAsset
    {
        public LoadedAnimation(string path, float PixelPerUnit, int tileWidth, ModPlugin plugin, float animationSpeed)
        {
            pixelPerUnit = PixelPerUnit;
            Width = tileWidth;
            speed = animationSpeed;
            this.path = path + ".png";
            this.tileWidth = tileWidth;
            using (System.IO.Stream stream = plugin.ModAssembly.GetManifestResourceStream(this.path))
            {
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    data = memoryStream.ToArray();
                }
            }
            anim = new SpriteSheet();
            anim.SpriteChangeSpeed = speed;
            anim.Sprites = new Sprite[] { };
            Assets.Add(this);
        }
        internal string path;
        internal float pixelPerUnit;
        internal int Width;
        internal byte[] data;
        internal int tileWidth;
        internal float speed;
        internal SpriteSheet anim;
        internal void Load()
        {
            if (parent == null)
            {
                parent = new GameObject("Loaded Animations").transform.DontDestroy();
            }
            List<Sprite> sprites = new List<Sprite>();
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            int tileHeight = texture.height;
            int totalTiles = 
            texture.width / tileWidth;
            Transform p = new GameObject(path).transform;
            p.SetParent(parent);
            p.gameObject.SetActive(false);
            for (int i = 0; i < totalTiles; i++)
            {
                SpriteRenderer storage = new GameObject(i.ToString()).AddComponent<SpriteRenderer>();
                storage.transform.SetParent(p);
                storage.sprite = Sprite.Create(texture, new Rect(i * tileWidth, 0, tileWidth, tileHeight), new Vector2(0.5f, 0.5f), pixelPerUnit);
                sprites.Add(storage.sprite);
            }
            anim.Sprites = sprites.ToArray();
        }
        public override SpriteSheet GetAsset()
        {
            foreach (Sprite sprite in anim.Sprites)
            {
                if (sprite.IsNullOrDestroyed())
                {
                    anim.Sprites = new Sprite[] { };
                    break;
                }
            }
            if (anim.Sprites.Count() <= 0)
            {
                Load();
            }
            return anim;
        }
        internal static Transform parent;
    }
}
