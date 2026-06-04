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
    public class CustomVisor : BaseCosmetic
    {
        public bool BehindHats = false;

        private Action<VisorViewData> __configureData;
        public CustomVisor(StringNames hatName, string ownerPlugin, Action<VisorViewData> configureData, Action<PreviewViewData> configurePreview = null)
            : base(hatName, ownerPlugin, configurePreview)
        {
            __configureData = configureData;
        }
        public override CosmeticData CreateData() => ScriptableObject.CreateInstance<VisorData>().DontUnload();
        public override void Initialize()
        {
            base.Initialize();

            if (Data != null) return;

            VisorData data = Data.SafeCast<VisorData>();

            data.behindHats = BehindHats;

            Data.ProductId = GetProductId("visor");

            VisorViewData viewData = ScriptableObject.CreateInstance<VisorViewData>().DontUnload();
            __configureData(viewData);

            data.PreviewCrewmateColor = viewData.MatchPlayerColor;

            SetPreview(viewData.IdleFrame);

            data.ViewDataRef = CosmeticManager.Register(viewData, $"ViewData.{Data.ProductId}");
        }
    }
}
