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

namespace FungleAPI.GameOptions.Collections
{
    public abstract class OptionCollection
    {
        public bool Dirty;
        public string FilePath;
        public ModPlugin Plugin;
        public List<IModdedOption> Options = new List<IModdedOption>();

        public string FolderName;
        public void Save()
        {
            if (!Dirty) return;

            using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    WriteLocalOptions(binaryWriter);
                    binaryWriter.Flush();
                    fileStream.Flush(true);
                }
            }
            Dirty = false;
        }
        public void Load()
        {
            if (!File.Exists(FilePath))
            {
                SetAsDefault(true);
                return;
            }
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    ReadLocalOptions(binaryReader);
                }
            }
        }

        public virtual void Initialize(ModPlugin modPlugin, List<IModdedOption> moddedOptions)
        {
            Plugin = modPlugin;
            Options.AddRange(moddedOptions);

            Type type = GetType();
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, FolderName), $"{type.Name}_{type.GetShortUniqueId()}");

            modPlugin.OptionCollections.Add(this);
            OptionManager.OptionCollections.Add(this);
            foreach (IModdedOption moddedOption in Options)
            {
                moddedOption.SetOnValueChance(delegate (bool changed)
                {
                    if (changed)
                    {
                        Dirty = true;
                    }
                    if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.Plugin == modPlugin) 
                    {
                        LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                    }
                });
                OptionManager.AllOptions.Add(moddedOption.OptionId, moddedOption);
            }

            Load();
        }
        public virtual void WriteLocalOptions(BinaryWriter binaryWriter) 
        {
            binaryWriter.Write(Options.Count);
            foreach (IModdedOption moddedOption in Options)
            {
                binaryWriter.Write(moddedOption.StringOptionId);
                moddedOption.WriteLocalValue(binaryWriter);
            }
        }
        public virtual void ReadLocalOptions(BinaryReader binaryReader) 
        {
            int optionCount = binaryReader.ReadInt32();
            for (int i = 0; i < optionCount; i++)
            {
                string optionId = binaryReader.ReadString();
                IModdedOption moddedOption = Options.FirstOrDefault(m => m.StringOptionId == optionId);
                if (moddedOption != null)
                {
                    moddedOption.ReadLocalValue(binaryReader);
                }
            }
        }
        public virtual void SetAsDefault(bool amHost)
        {
            foreach (IModdedOption moddedOption in Options)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, amHost);
            }
        }
    }
}
