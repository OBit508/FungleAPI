using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.LoadMod;
using FungleAPI.Patches;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Assets
{
    public class LoadedSprite : LoadedAsset
    {
        public LoadedSprite(string path, float pixelPerUnit, ModPlugin plugin)
        {
            this.path = path;
            this.pixelPerUnit = pixelPerUnit;
            this.path = path + ".png";
            System.IO.Stream manifestResourceStream = plugin.ModAssembly.GetManifestResourceStream(this.path);
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                manifestResourceStream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
            }
        }
        internal byte[] data;
        internal string path;
        internal float pixelPerUnit;
        internal Sprite sprite;
        public override Sprite GetAsset()
        {
            if (sprite == null)
            {
                if (parent == null)
                {
                    parent = new GameObject("All Sprites").transform.DontDestroy();
                    parent.gameObject.SetActive(false);
                }
                SpriteRenderer storage = new GameObject(path).AddComponent<SpriteRenderer>();
                storage.transform.SetParent(parent);
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture2D.LoadImage(data);
                storage.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), pixelPerUnit);
                sprite = storage.sprite;
            }
            return sprite;
        }
        internal static Transform parent;
    }
}
