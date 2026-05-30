using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Assets;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility
{
    public class SubmergedCompatibility
    {
        public static SubmergedCompatibility Instance;
        public Type SubmarineStatus { get; protected set; }
        public virtual void Initialize() { }
        public virtual System.Collections.IEnumerator CoWaitMapLoader() { yield return null; }

        public static void CheckSubmerged()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("Submerged", out _))
            {
                using (Stream stream = FungleApiPlugin.Plugin.ModAssembly.GetManifestResourceStream("FungleAPI.ModCompatibility.DLLs.SubWithFungle.dll"))
                {
                    Assembly assembly = Assembly.Load(stream.ToArray());

                    Instance = (SubmergedCompatibility)Activator.CreateInstance(assembly.GetType("SubWithFungle.SFCompatibility"));

                    Instance.Initialize();
                }
            }
        }
    }
}
