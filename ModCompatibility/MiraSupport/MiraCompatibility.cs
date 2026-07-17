using BepInEx.Unity.IL2CPP;
using FungleAPI.Api;
using FungleAPI.Assets;
using FungleAPI.Event;
using FungleAPI.Freeplay.Helpers;
using FungleAPI.ModCompatibility.ReactorSupportTemp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.ModCompatibility.MiraSupport
{
    public class MiraCompatibility
    {
        public static MiraCompatibility Instance;
        public virtual MiraRoleExtensions RoleExtensions => null;
        public virtual MiraConfigs RoleConfigs => null;
        public virtual void Initialize() { }
        public virtual IEnumerable<RoleBehaviour> CompleteRoleRegistration() => null;
        public virtual void PopulateMiraLobbyTabs() { }
        public virtual bool MiraGameOverActive() => false;
        public virtual void AssignModifiers() { }
        public virtual bool IsMiraAssembly(Assembly assembly) => false;
        public virtual void CreateMiraFolders(TaskAdderGame taskAdderGame, Dictionary<TaskFolder, List<FolderItem>> folders) { }

        public static void CheckMira()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("mira.api", out _))
            {
                using (Stream stream = FungleApiPlugin.Plugin.ModAssembly.GetManifestResourceStream("FungleAPI.ModCompatibility.DLLs.MiraWithFungle.dll"))
                {
                    Assembly assembly = Assembly.Load(stream.ToArray());

                    Instance = (MiraCompatibility)Activator.CreateInstance(assembly.GetType("MiraWithFungle.MFCompatibility"));

                    Instance.Initialize();

                    EventManager.RegisterEvents(assembly.GetTypes());
                }
            }
        }
    }
}
