using BepInEx.Unity.IL2CPP;
using FungleAPI.Event;
using FungleAPI.Event.BelpInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Assets;
using FungleAPI.PluginLoading;
using Iced.Intel;

namespace FungleAPI.ModCompatibility.ReactorSupportTemp
{
    public abstract class ReactorCompatibility
    {
        public static ReactorCompatibility Instance;
        public virtual void Initialize() { }
        public abstract StringNames CreateStringName();
        public abstract void Register(string name, string version, bool isPreRelease, Func<ReactorCreditsLocation, bool> shouldShow);

        public static void CheckReactor()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.reactor.api", out _))
            {
                using (Stream stream = FungleAPIPlugin.Plugin.ModAssembly.GetManifestResourceStream("FungleAPI.ModCompatibility.DLLs.ReactorWithFungle.dll"))
                {
                    Assembly assembly = Assembly.Load(stream.ToArray());

                    Instance = (ReactorCompatibility)Activator.CreateInstance(assembly.GetType("ReactorWithFungle.RFCompatibility"));

                    Instance.Initialize();

                    Instance.Register("FungleAPI", FungleAPIPlugin.ModV, false, (p) => p == ReactorCreditsLocation.PingTracker);
                }
            }
        }
    }
}
