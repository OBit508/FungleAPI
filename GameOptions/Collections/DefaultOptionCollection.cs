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
using static Il2CppSystem.Net.WebSockets.ManagedWebSocket;

namespace FungleAPI.GameOptions.Collections
{
    public class DefaultOptionCollection : OptionCollection
    {
        public const int DefaultOptionVersion = 3;

        public override void WriteLocalOptions(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(DefaultOptionVersion);
            base.WriteLocalOptions(binaryWriter);
        }
        public override void ReadLocalOptions(BinaryReader binaryReader)
        {
            SetAsDefault(true);
            try
            {
                int defaultOptionVersion = binaryReader.ReadInt32();
                if (defaultOptionVersion != DefaultOptionVersion)
                {
                    FungleApiPlugin.Instance.Log.LogWarning($"Different version of the Default Option Collection from {FilePath} founded, loading default.");
                    return;
                }
                base.ReadLocalOptions(binaryReader);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Default Option Collection from {FilePath}, loading default.\nMessage: {ex.Message}");
                SetAsDefault(true);
            }
        }
        public DefaultOptionCollection(string folderName)
        {
            FolderName = folderName;
        }
    }
}
