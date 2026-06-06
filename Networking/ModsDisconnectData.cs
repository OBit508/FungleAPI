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
            if (messageReader.ReadBoolean())
            {
                MissingMods = messageReader.ReadString();
            }
            if (messageReader.ReadBoolean())
            {
                ExtraMods = messageReader.ReadString();
            }
        }
    }
}
