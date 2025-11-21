using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.Configuration.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration
{
    [FungleIgnore]
    public class ModSettings
    {
        public List<ModdedOption> Options = new List<ModdedOption>();
        public List<SettingsGroup> Groups = new List<SettingsGroup>();
        public bool initialized;
        public virtual void Initialize()
        {
            if (!initialized)
            {
                Type type = GetType();
                foreach (Type t in type.GetNestedTypes())
                {
                    if (typeof(SettingsGroup).IsAssignableFrom(t))
                    {
                        SettingsGroup group = (SettingsGroup)Activator.CreateInstance(t);
                        group.Initialize();
                        Options.AddRange(group.Options);
                        Groups.Add(group);
                    }
                }
                initialized = true;
            }
        }
    }
}
