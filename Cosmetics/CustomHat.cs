using FungleAPI.Extensions;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class CustomHat : BaseCosmetic
    {
        public bool BlocksVisors = false;
        public bool NoBounce = false;
        public bool InFront = true;

        private Action<HatViewData> __configureData;
        public CustomHat(StringNames hatName, string ownerPlugin, Action<HatViewData> configureData, Action<PreviewViewData> configurePreview = null)
            :base(hatName, ownerPlugin, configurePreview)
        {
            __configureData = configureData;
        }
        public override CosmeticData CreateData() => ScriptableObject.CreateInstance<HatData>().DontUnload();
        public override void Initialize()
        {
            base.Initialize();

            if (Data != null) return;

            HatData data = Data.SafeCast<HatData>();

            data.BlocksVisors = BlocksVisors;
            data.NoBounce = NoBounce;
            data.InFront = InFront;

            Data.ProductId = GetProductId("hat");

            HatViewData viewData = ScriptableObject.CreateInstance<HatViewData>().DontUnload();
            __configureData(viewData);

            data.PreviewCrewmateColor = viewData.MatchPlayerColor;

            SetPreview(viewData.MainImage);

            data.ViewDataRef = CosmeticManager.Register(viewData, $"ViewData.{Data.ProductId}");
        }
    }
}
