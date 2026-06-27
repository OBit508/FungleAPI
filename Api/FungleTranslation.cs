using FungleAPI.Translation;
using Il2CppSystem.Runtime.Remoting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Api
{
    /// <summary>
    /// FungleAPI translations
    /// </summary>
    [TranslationAttribute("FungleAPI.Assets.FungleTranslations")]
    public static class FungleTranslation
    {
        public static Translator RoomText { get; } = new Translator("Room");
        public static Translator RoomSettings { get; } = new Translator("Room Settings");
        public static Translator RoomTabDescription { get; } = new Translator("Edit the Room settings for your lobby.");
        public static Translator PluralGameModeText { get; } = new Translator("Game Modes");
        public static Translator CheatingWarnText { get; } = new Translator("{0} is cheating.");
        public static Translator FungleCreditsText { get; } = new Translator("Thanks for using Fungle Api!!!");
        public static Translator ExileText { get; } = new Translator("{0} was an {1}.");
        public static Translator FailedToSync { get; } = new Translator("Failed to sync settings with host.\nError: ");
        public static Translator GameModeText { get; } = new Translator("Game Mode");
        public static Translator VictoryText { get; } = new Translator("Victory of the {0}");
        public static Translator ChanceText { get; } = new Translator("Chance");
        public static Translator PriorityText { get; } = new Translator("Priority");
        public static Translator ChangeToPublicText { get; } = new Translator("Public rooms are disabled for security reasons.");
        public static Translator LoadingShipPrefabsText { get; } = new Translator("Loading Ship Prefabs");
        public static Translator LoadingAssetsText { get; } = new Translator("Loading Mods Assets");
        public static Translator YourRoleIsText { get; } = new Translator("Your role is {0}");
        public static Translator EditText { get; } = new Translator("Edit preset");
        public static Translator CountText { get; } = new Translator("Player Count");
        public static Translator TeamPriorityText { get; } = new Translator("Team Priority");
        public static Translator ImpostorsText { get; } = new Translator("Impostors");
        public static Translator NeutralText { get; } = new Translator("Neutral");
        public static Translator NeutralsText { get; } = new Translator("Neutrals");
        public static Translator TeamsRemainText { get; } = new Translator("Remaining Teams: {0}");
        public static Translator NeutralGameOver { get; } = new Translator("Victory of the Neutrals");
        public static Translator ImpostorGameOver { get; } = new Translator("Victory of the Impostors");
        public static Translator CrewmateGameOver { get; } = new Translator("Crew Victory");
        public static Translator TeamsText { get; } = new Translator("Teams");
        public static Translator TeamConfigButtonText { get; } = new Translator("Teams Settings");
        public static Translator TeamConfigDescText { get; } = new Translator("Edit the Teams settings for your lobby.");
        public static Translator TypeHereText { get; } = new Translator("Type here who was the killer");

        public static Translator HandShakeFail_MissingAPIDisconnect { get; } = new Translator("{0} does not have the FungleAPI.");
        public static Translator HandShakeFail_MissingMods { get; } = new Translator("The following mods are missing from the local client: {0}");
        public static Translator HandShakeFail_ExtraMods { get; } = new Translator("The following mods are missing from the host client: {0}");
        public static Translator HandShakeFail_HostNotModded { get; } = new Translator("Host is not modded");
    }
}
