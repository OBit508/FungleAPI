using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.AntiCheat;
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
            if (innerNetObject == null) return;

            System.Collections.IEnumerator CoHandleRpc()
            {
                while (innerNetObject == null || innerNetObject != null && (innerNetObject.Data == null || innerNetObject.Data.ClientId < 0)) yield return null;

                if (innerNetObject == null) yield break;

                if (AntiCheatManager.Active && !innerNetObject.Data.IsHost())
                {
                    AntiCheatManager.CheaterFinded(innerNetObject.Data.ClientId);

                    yield break;
                }

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
            Helpers.StartCoroutine(CoHandleRpc().WrapToIl2Cpp());
        }
    }
}
