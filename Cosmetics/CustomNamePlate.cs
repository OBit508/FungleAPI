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
    public class CustomNamePlate : BaseCosmetic
    {
        private Action<NamePlateViewData> __configureData;
        public CustomNamePlate(StringNames hatName, string ownerPlugin, Action<NamePlateViewData> configureData, Action<PreviewViewData> configurePreview = null)
            : base(hatName, ownerPlugin, configurePreview)
        {
            __configureData = configureData;
        }
        public override CosmeticData CreateData() => ScriptableObject.CreateInstance<NamePlateData>().DontUnload();
        public override void Initialize()
        {
            base.Initialize();

            if (Data != null) return;

            NamePlateData data = Data.SafeCast<NamePlateData>();

            Data.ProductId = GetProductId("nameplate");

            NamePlateViewData viewData = ScriptableObject.CreateInstance<NamePlateViewData>().DontUnload();
            __configureData(viewData);

            SetPreview(viewData.Image);

            data.ViewDataRef = CosmeticManager.Register(viewData, $"ViewData.{Data.ProductId}");
        }
    }
}
