using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using Hazel;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Player.Networking
{
    internal class RpcSendMods : SimpleRpc<PlayerControl>
    {
        public override void Write(MessageWriter messageWriter)
        {
            messageWriter.Write((ushort)HandShakeManager.RequiredMods.Count);

            foreach (BepInMod bepInMod in HandShakeManager.RequiredMods.Values)
            {
                messageWriter.Write(bepInMod.GUID);
                messageWriter.Write(bepInMod.Version);
                messageWriter.Write(bepInMod.Name);
            }
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            ushort modsCount = messageReader.ReadUInt16();

            BepInMod[] mods = new BepInMod[modsCount];

            for (int i = 0; i < modsCount; i++)
            {
                string GUID = messageReader.ReadString();
                string version = messageReader.ReadString();
                string name = messageReader.ReadString();
                mods[i] = new BepInMod() { GUID = GUID, Version = version, Name = name };
            }

            HandShakeHelper handShakeHelper = innerNetObject.gameObject.GetOrAddComponent<HandShakeHelper>();
            handShakeHelper.Owner = innerNetObject;

            HandShakeManager.GetMods(mods, out handShakeHelper.MissingMods, out handShakeHelper.MissingOnClientMods);

            handShakeHelper.SameMods = HandShakeManager.RequiredMods.Values.ToList().FindAll(m => !handShakeHelper.MissingMods.Any(r => r.GUID == m.GUID));

            handShakeHelper.DidHandShake = true;
        }
    }
}
