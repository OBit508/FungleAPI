using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Lobby
{
    public abstract class LobbyTab
    {
        public ModPlugin Plugin;

        public PassiveButton ViewSettingsButton;
        public PassiveButton EditSettingsButton;

        public abstract string ViewTabButtonText { get; }
        public abstract string EditTabButtonText { get; }

        public abstract string TabDescriptionText { get; }

        public abstract void BuildEditTab(GameOptionsMenu gameOptionsMenu);
        public abstract void BuildViewTab(LobbyViewSettingsPane lobbyViewSettingsPane);
    }
}
