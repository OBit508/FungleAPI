using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncOptions : AdvancedRpc<IEnumerable<IModdedOption>, PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, IEnumerable<IModdedOption> data)
        {
            messageWriter.WritePacked(data.Count());
            foreach (IModdedOption moddedOption in data)
            {
                messageWriter.WriteOption(moddedOption);
                moddedOption.Serialize(messageWriter);
            }
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            try
            {
                uint optionsCount = messageReader.ReadPackedUInt32();
                for (int i = 0; i < optionsCount; i++)
                {
                    IModdedOption moddedOption = messageReader.ReadOption();
                    moddedOption.Deserialize(messageReader);
                }
            }
            catch (Exception ex)
            {
                HandShakeManager.DisconnectWithReason(FungleTranslation.FailedToSync.GetString() + ex.Message);
            }
        }
    }
}
