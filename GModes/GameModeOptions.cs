using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Options;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GModes
{
    public class GameModeOptions
    {
        public List<SettingsGroup> Groups = new List<SettingsGroup>();
        public ModeOptionCollection OptionCollection;
        public bool initialized;
        public virtual void Initialize(ModPlugin modPlugin)
        {
            if (!initialized)
            {
                Type type = GetType();
                foreach (Type t in type.GetNestedTypes())
                {
                    if (typeof(SettingsGroup).IsAssignableFrom(t))
                    {
                        SettingsGroup group = (SettingsGroup)Activator.CreateInstance(t);
                        group.Initialize(modPlugin);
                        Groups.Add(group);
                    }
                }
                OptionCollection = new ModeOptionCollection(this);
                OptionCollection.Initialize(type, modPlugin);
                initialized = true;
            }
        }
    }
}
