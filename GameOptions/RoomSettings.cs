using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.GameOptions.Collections;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions
{
    /// <summary>
    /// Class used to create the room tab settings
    /// </summary>
    [FungleIgnore]
    public class RoomSettings
    {
        public List<SettingsGroup> Groups = new List<SettingsGroup>();
        public DefaultOptionCollection OptionCollection;
        public bool initialized;
        public virtual void Initialize(ModPlugin modPlugin)
        {
            if (!initialized)
            {
                List<IModdedOption> moddedOptions = new List<IModdedOption>();

                Type type = GetType();
                foreach (Type t in type.GetNestedTypes())
                {
                    if (t.ShouldIgnore()) continue;

                    if (typeof(SettingsGroup).IsAssignableFrom(t))
                    {
                        SettingsGroup group = (SettingsGroup)Activator.CreateInstance(t);
                        group.Initialize(modPlugin);

                        moddedOptions.AddRange(group.Options);

                        Groups.Add(group);
                    }
                }
                OptionCollection = new DefaultOptionCollection("Room");
                OptionCollection.Initialize(modPlugin, moddedOptions);
                initialized = true;
            }
        }
    }
}
