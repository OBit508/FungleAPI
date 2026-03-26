using FungleAPI.Base.Rpc;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Configuration.Networking
{
    internal class RpcSyncGameOpt : AdvancedRpc<IEnumerable<ModPlugin>>
    {
        public override void Write(MessageWriter messageWriter, IEnumerable<ModPlugin> data)
        {
            messageWriter.WritePacked(data.Count());
            foreach (ModPlugin modPlugin in data)
            {
                messageWriter.WritePacked(modPlugin.Settings.Options.Count);
                foreach (ModdedOption moddedOption in modPlugin.Settings.Options)
                {
                    messageWriter.WriteOption(moddedOption);
                    messageWriter.Write(moddedOption.localValue.Value);
                }
            }
        }
        public override void Handle(MessageReader messageReader)
        {
            int gameOptCount = messageReader.ReadPackedInt32();
            for (int i = 0; i < gameOptCount; i++)
            {
                int optionCount = messageReader.ReadPackedInt32();
                int x = 0;
                while (x < optionCount)
                {
                    ModdedOption moddedOption = messageReader.ReadOption();
                    moddedOption.onlineValue = messageReader.ReadString();
                    x++;
                }
            }
        }
    }
}
