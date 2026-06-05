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
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.GameOptions.Collections
{
    public class DefaultOptionCollection : OptionCollection
    {
        public const int DefaultOptionVersion = 2;

        public FolderType Folder;

        public override void Initialize(Type type, ModPlugin modPlugin)
        {
            Plugin = modPlugin;
            CollectionId = $"{type.Name}.{type.GetShortUniqueId()}";
            FilePath = Path.Combine(FileManager.GetFolder(modPlugin, Folder), $"{CollectionId}.funglecfg");
            foreach (IModdedOption moddedOption in OptionManager.GetAndInitializeModdedOptions(type, modPlugin))
            {
                moddedOption.SetOnValueChance((bool changed) => { if (changed) { Dirty = true; } });
                OptionManager.AllOptions.Add(moddedOption.OptionId, moddedOption);
                Options.Add(moddedOption.OptionId, moddedOption);
            }
            ReadLocalOptions();
        }
        public override void WriteLocalOptions()
        {
            using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(DefaultOptionVersion);

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
                SetAsDefault();
                return;
            }
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(FilePath)))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                    {
                        int defaultOptionVersion = binaryReader.ReadInt32();
                        if (defaultOptionVersion < DefaultOptionVersion)
                        {
                            FungleApiPlugin.Instance.Log.LogWarning($"Newer version of the Default Option Collection from {FilePath} founded, loading and saving default.");
                            SetAsDefault();
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
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Default Option Collection from {FilePath}, loading and saving default.\nMessage: {ex.Message}");
                SetAsDefault();
            }
        }
        public void SyncNonHostWithLocal()
        {
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SyncNonHostWithLocal();
            }
        }
        public void SetAsDefault()
        {
            foreach (IModdedOption moddedOption in Options.Values)
            {
                moddedOption.SetValue(moddedOption.DefaultValue, true);
            }
            SyncNonHostWithLocal();
        }
    }
}
