using FungleAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Rewired.Controller;

namespace FungleAPI.Cosmetics
{
    public class BaseCosmetic
    {
        private static Dictionary<string, uint> cosmetics = new Dictionary<string, uint>();

        public CosmeticData Data;
        public StringNames CosmeticName;
        public string OwnerPlugin;

        protected Action<PreviewViewData> __configurePreview;
        
        public BaseCosmetic(StringNames cosmeticName, string ownerPlugin, Action<PreviewViewData> configurePreview = null)
        {
            CosmeticName = cosmeticName;
            OwnerPlugin = ownerPlugin;
            __configurePreview = configurePreview;
        }

        public string GetProductId(string item)
        {
            string prodId = $"{item}_{OwnerPlugin}";

            if (cosmetics.TryGetValue(OwnerPlugin, out uint value))
            {
                prodId += value;
                cosmetics[OwnerPlugin] = value + 1;
            }
            else
            {
                cosmetics[OwnerPlugin] = 1;
            }
            return prodId;
        }
        public virtual CosmeticData CreateData() { return null; }
        public virtual void Initialize()
        {
            if (Data != null) return;

            Data = CreateData();

            CosmeticManager.CosmeticsNames.Add(Data, CosmeticName);

            Data.Free = true;
            Data.NotInStore = true;
        }
        public void SetPreview(Sprite image)
        {
            PreviewViewData previewViewData = ScriptableObject.CreateInstance<PreviewViewData>().DontUnload();

            if (__configurePreview == null)
            {
                previewViewData.PreviewSprite = image;
            }
            else
            {
                __configurePreview(previewViewData);
            }

            Data.PreviewData = CosmeticManager.Register(previewViewData, $"PreviewData.{Data.ProductId}");
        }
    }
}
