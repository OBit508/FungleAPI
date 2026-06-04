using FungleAPI.Extensions;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetics
{
    public class CustomSkin : BaseCosmetic
    {
        private Action<SkinViewData> __configureData;
        public CustomSkin(StringNames hatName, string ownerPlugin, Action<SkinViewData> configureData, Action<PreviewViewData> configurePreview = null)
            : base(hatName, ownerPlugin, configurePreview)
        {
            __configureData = configureData;
        }
        public override CosmeticData CreateData() => ScriptableObject.CreateInstance<SkinData>().DontUnload();
        public override void Initialize()
        {
            base.Initialize();

            if (Data != null) return;

            SkinData data = Data.SafeCast<SkinData>();

            Data.ProductId = GetProductId("skin");

            SkinViewData viewData = ScriptableObject.CreateInstance<SkinViewData>().DontUnload();
            __configureData(viewData);

            data.PreviewCrewmateColor = viewData.MatchPlayerColor;

            SetPreview(viewData.IdleFrame);

            data.ViewDataRef = CosmeticManager.Register(viewData, $"ViewData.{Data.ProductId}");
        }
    }
}
