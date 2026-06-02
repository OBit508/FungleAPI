using FungleAPI.Extensions;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics.Hats
{
    public class CustomHat
    {
        private static Dictionary<string, uint> hats = new Dictionary<string, uint>();

        public HatData HatData;
        public StringNames HatName;
        public string OwnerPlugin;

        public bool BlocksVisors = false;
        public bool NoBounce = false;
        public bool InFront = true;

        private Func<HatViewData> __hatViewDataFactory;
        private Func<PreviewViewData> __previewViewDataFactory;
        public CustomHat(StringNames hatName, string ownerPlugin, Func<HatViewData> hatViewDataFactory, Func<PreviewViewData> previewViewDataFactory = null)
        {
            HatName = hatName;
            OwnerPlugin = ownerPlugin;
            __hatViewDataFactory = hatViewDataFactory;
            __previewViewDataFactory = previewViewDataFactory;
        }

        public void Initialize()
        {
            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Initializing");

            if (HatData != null) return;

            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Creating HatData");
            HatData = ScriptableObject.CreateInstance<HatData>().DontUnload();

            CosmeticManager.CosmeticsNames.Add(HatData, HatName);

            HatData.Free = true;
            HatData.NotInStore = true;
            HatData.BlocksVisors = BlocksVisors;
            HatData.NoBounce = NoBounce;
            HatData.InFront = InFront;

            HatData.ProductId = $"hat_{OwnerPlugin}";

            if (hats.TryGetValue(OwnerPlugin, out uint value))
            {
                HatData.ProductId += value;
                hats[OwnerPlugin] = value + 1;
            }
            else
            {
                hats[OwnerPlugin] = 1;
            }

            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Creating HatViewData");
            HatViewData hatViewData = __hatViewDataFactory().DontUnload();

            HatData.PreviewCrewmateColor = hatViewData.MatchPlayerColor;

            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Creating PreviewViewData");
            PreviewViewData previewViewData;
            if (__previewViewDataFactory == null)
            {
                previewViewData = ScriptableObject.CreateInstance<PreviewViewData>().DontUnload();
                previewViewData.PreviewSprite = hatViewData.MainImage;
            }
            else
            {
                previewViewData = __previewViewDataFactory().DontUnload();
            }

            FunglePlugin<FungleApiPlugin>.Logger.LogInfo("Creating AssetReferences");
            HatData.ViewDataRef = CosmeticManager.Register(hatViewData, $"hatviewdata.{HatData.ProductId}");
            HatData.PreviewData = CosmeticManager.Register(previewViewData, $"previewviewdata.{HatData.ProductId}");
        }
    }
}
