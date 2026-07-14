using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Lobby
{
    public abstract class FungleTab : LobbyTab
    {
        public ModPlugin Plugin => ModPluginManager.GetModPlugin(TabAssembly);
    }
}
