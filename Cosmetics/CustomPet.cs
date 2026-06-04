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
    public class CustomPet : BaseCosmetic
    {
        private Func<PetBehaviour> __createPet;
        private Func<Sprite> __previewSprite;
        public CustomPet(StringNames hatName, string ownerPlugin, Func<PetBehaviour> createPet, Func<Sprite> previewSprite, Action<PreviewViewData> configurePreview = null)
            : base(hatName, ownerPlugin, configurePreview)
        {
            __createPet = createPet;
            __previewSprite = previewSprite;
        }
        public override CosmeticData CreateData() => ScriptableObject.CreateInstance<PetData>().DontUnload();
        public override void Initialize()
        {
            base.Initialize();

            if (Data != null) return;

            PetData data = Data.SafeCast<PetData>();

            Data.ProductId = GetProductId("skin");

            PetBehaviour petBehaviour = __createPet();

            SetPreview(__previewSprite());

            data.PetPrefabRef = CosmeticManager.Register(petBehaviour, $"ViewData.{Data.ProductId}");
        }
    }
}
