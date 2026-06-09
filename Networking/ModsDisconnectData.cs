using Hazel;
using Il2CppSystem.Runtime.Remoting.Messaging;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Networking
{
    internal struct ModsDisconnectData
    {
        public string MissingMods;
        public string ExtraMods;

        public ModsDisconnectData(string missingMods, string extraMods)
        {
            MissingMods = missingMods;
            ExtraMods = extraMods;
        }
        public ModsDisconnectData(MessageReader messageReader)
        {
            byte b = messageReader.ReadByte();

            if (b == 0 || b == 2)
            {
                MissingMods = messageReader.ReadString();
            }
            if (b == 1 || b == 2)
            {
                ExtraMods = messageReader.ReadString();
            }
        }
    }
}
