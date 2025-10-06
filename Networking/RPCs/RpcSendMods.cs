using FungleAPI.Components;
using FungleAPI.Utilities;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking.RPCs
{
    public class RpcSendMods : CustomRpc<ClientData>
    {
        public override void Write(MessageWriter writer, ClientData value)
        {
            List<ModPlugin.Mod> mods = new List<ModPlugin.Mod>();
            writer.Write(value.Id);
            writer.Write(ModPlugin.AllPlugins.Count);
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                writer.WriteMod(plugin.LocalMod);
                mods.Add(plugin.LocalMod);
            }
            Helpers.Mods.TryAdd(value, mods);
        }
        public override void Handle(MessageReader reader)
        {
            List<ModPlugin.Mod> mods = new List<ModPlugin.Mod>();
            int clientId = reader.ReadInt32();
            int modCount = reader.ReadInt32();
            for (int i = 0; i < modCount; i++)
            {
                mods.Add(reader.ReadMod());
            }
            ClientData client = AmongUsClient.Instance.GetClient(clientId);
            if (client != null)
            {
                if (Helpers.Mods.TryAdd(client, mods) && AmongUsClient.Instance.AmHost)
                {
                    ModPlugin.Mod.Update(client);
                }
            }
        }
    }
}
