using BepInEx.Configuration;
using FungleAPI.Api;
using FungleAPI.GameOptions.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FungleAPI.GameOptions.Collections
{
    public class OptionCollection
    {
        private static readonly object ConfigLock = new object();

        public ModPlugin Plugin;
        public List<IModdedOption> Options = new List<IModdedOption>();

        public Type OwnerType;
        public string CategoryName;

        public virtual void Initialize(ModPlugin modPlugin, List<IModdedOption> moddedOptions)
        {
            Plugin = modPlugin;
            Options.AddRange(moddedOptions);

            string collectionId = $"{CategoryName}_{OwnerType.Name}_{OwnerType.GetShortUniqueId()}";

            ConfigFile configFile = FileManager.GetFile(modPlugin);

            lock (ConfigLock)
            {
                modPlugin.OptionCollections.Add(this);

                foreach (IModdedOption moddedOption in Options)
                {
                    moddedOption.Entry = configFile.Bind( collectionId, moddedOption.StringOptionId, moddedOption.DefaultValue.ToString());

                    moddedOption.SetOnValueChance(delegate
                    {
                        if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == modPlugin.ModAssembly)
                        {
                            LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                        }
                    });

                    OptionManager.AllOptions[moddedOption.OptionId] = moddedOption;
                }

                try
                {
                    LoadOptions(configFile, collectionId);
                }
                catch
                {
                    FunglePlugin<FungleApiPlugin>.Logger.LogError( "Found a corrupted ConfigEntry value from Options Collections, loading default.");
                    SetAsDefault(true);
                }
            }
        }
        public virtual void LoadOptions(ConfigFile configFile, string collectionId) 
        {
            foreach (IModdedOption moddedOption in Options)
            {
                moddedOption.LoadValue(moddedOption.Entry);
            }
        }
        public virtual void SetAsDefault(bool amHost)
        {
            foreach (IModdedOption moddedOption in Options)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, amHost);
            }
        }
        public OptionCollection(string categoryName, Type ownerType)
        {
            CategoryName = categoryName;
            OwnerType = ownerType;
        }
    }
}
