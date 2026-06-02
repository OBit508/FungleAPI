using Cpp2IL.Core.Extensions;
using FungleAPI.Attributes;
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace FungleAPI.Cosmetics.Helpers
{
    internal class CosmeticLocator : Il2CppSystem.Object
    {
        private static CosmeticLocator _instance;
        private static IResourceLocator _locator;

        [EventRegister]
        public static void Initialize(FinishedPluginLoadingEvent e)
        {
            _instance = new CosmeticLocator();
            _locator = new IResourceLocator(_instance.Pointer);
            Addressables.AddResourceLocator(_locator);
        }
        public CosmeticLocator(IntPtr ptr) : base(ptr) { }
        public CosmeticLocator() : base(ClassInjector.DerivedConstructorPointer<CosmeticLocator>()) => ClassInjector.DerivedConstructorBody(this);
        public string LocatorId => GetType().FullName;
        public Il2CppSystem.Collections.Generic.IEnumerable<Il2CppSystem.Object> Keys { get; } = Il2CppSystem.Array.Empty<Il2CppSystem.Object>().SafeCast<Il2CppSystem.Collections.Generic.IEnumerable<Il2CppSystem.Object>>();
        private static string ProviderId => typeof(CosmeticProvider).FullName;
        public bool Locate(Il2CppSystem.Object key, Il2CppSystem.Type type, out Il2CppSystem.Collections.Generic.IList<IResourceLocation> locations)
        {
            locations = null;

            if (key == null) return false;

            string id = key.ToString();

            if (id == null) return false;

            if (!CosmeticManager.Assets.TryGetValue(id, out UnityEngine.Object obj)) return false;

            Il2CppSystem.Type il2CppType = Il2CppType.From(obj.GetType());

            ResourceLocationBase location = new ResourceLocationBase(id, id, ProviderId, il2CppType);

            Il2CppSystem.Collections.Generic.List<ResourceLocationBase> list = new Il2CppSystem.Collections.Generic.List<ResourceLocationBase>();
            list.Add(location);
            locations = new Il2CppSystem.Collections.Generic.IList<IResourceLocation>(list.Pointer);

            return true;
        }
    }
}
