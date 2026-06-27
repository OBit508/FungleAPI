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

namespace FungleAPI.GameOptions.Collections
{
    public class GameOptionCollection : OptionCollection
    {
        public const int GameOptionVersion = 2;

        public ModSettings Settings;

        public override void Initialize(Type type, ModPlugin modPlugin)
        {
            Plugin = modPlugin;
            CollectionId = $"{type.Name}.{type.GetShortUniqueId()}";
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, FolderType.GameSettings), $"{CollectionId}.funglecfg");

            List<IModdedOption> EveryOption = new List<IModdedOption>();
            foreach (SettingsGroup settingsGroup in Settings.Groups)
            {
                EveryOption.AddRange(settingsGroup.Options);
            }

            foreach (IModdedOption moddedOption in EveryOption)
            {
                moddedOption.SetOnValueChance((bool changed) => { if (changed) { Dirty = true; } });
                OptionManager.AllOptions.Add(moddedOption.OptionId, moddedOption);
                Options.Add(moddedOption.OptionId, moddedOption);
            }


            modPlugin.OptionCollections.Add(this);
            OptionManager.OptionCollections.Add(this);

            ReadLocalOptions();
        }
        public override void WriteLocalOptions()
        {
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(GameOptionVersion);

                    binaryWriter.Write(Options.Count);
                    foreach (IModdedOption moddedOption in Options.Values)
                    {
                        binaryWriter.Write(moddedOption.StringOptionId);
                        moddedOption.WriteLocalValue(binaryWriter);
                    }

                    binaryWriter.Flush();
                    fileStream.Flush(true);
                }
            }
        }
        public override void ReadLocalOptions()
        {
            if (!File.Exists(FilePath))
            {
                SetAsDefault(true);
                return;
            }
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(FilePath)))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        int gameOptionVersion = binaryReader.ReadInt32();
                        if (gameOptionVersion < GameOptionVersion)
                        {
                            FungleApiPlugin.Instance.Log.LogWarning($"Newer version of the Game Option Collection from {FilePath} founded, loading and saving default.");
                            SetAsDefault(true);
                            return;
                        }
                        int optionCount = binaryReader.ReadInt32();
                        for (int i = 0; i < optionCount; i++)
                        {
                            string optionId = binaryReader.ReadString();
                            IModdedOption moddedOption = Options.Values.FirstOrDefault(m => m.StringOptionId == optionId);
                            if (moddedOption != null)
                            {
                                moddedOption.ReadLocalValue(binaryReader);
                            }
                        }

                        SyncNonHostWithLocal();
                    }
                }
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Game Option Collection from {FilePath}, loading and saving default.\nMessage: {ex.Message}");
                SetAsDefault(true);
            }
        }
        public override void SyncNonHostWithLocal()
        {
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SyncNonHostWithLocal();
            }
        }
        public override void SetAsDefault(bool amHost)
        {
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, amHost);
            }


            if (amHost)
            {
                SyncNonHostWithLocal();
            }
        }
        public GameOptionCollection(ModSettings modSettings)
        {
            Settings = modSettings;
        }
    }
}
