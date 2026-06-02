using FungleAPI.Attributes;
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace FungleAPI.Cosmetics.Helpers
{
    internal class CosmeticProvider : ResourceProviderBase
    {
        private static CosmeticProvider _instance;
        private static IResourceProvider _provider;

        [EventRegister]
        public static void Initialize(FinishedPluginLoadingEvent finishedPluginLoadingEvent)
        {
            _instance = new CosmeticProvider();
            _provider = new IResourceProvider(_instance.Pointer);
            Addressables.ResourceManager.ResourceProviders.Insert(0, _provider);
        }
        public CosmeticProvider(IntPtr ptr) : base(ptr) { }
        public CosmeticProvider() : base(ClassInjector.DerivedConstructorPointer<CosmeticProvider>()) => ClassInjector.DerivedConstructorBody(this);
        public override bool CanProvide(Il2CppSystem.Type t, IResourceLocation location) => CosmeticManager.Assets.TryGetValue(location.InternalId, out _);
        public override Il2CppSystem.Type GetDefaultType(IResourceLocation location) => location.ResourceType;
        public override void Provide(ProvideHandle provideHandle)
        {
            string id = provideHandle.Location.InternalId;

            if (!CosmeticManager.Assets.TryGetValue(id, out UnityEngine.Object obj))
            {
                provideHandle.Complete<UnityEngine.Object>(null, false, new Il2CppSystem.Exception($"[RuntimeAssetProvider] Id not found: {id}"));
                return;
            }
            provideHandle.Complete(obj, true, null);
        }
        public override void Release(IResourceLocation location, Il2CppSystem.Object obj) { }
    }
}
