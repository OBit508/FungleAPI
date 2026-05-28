using Il2CppSystem.Runtime.Remoting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    /// <summary>
    /// FungleAPI translations
    /// </summary>
    public static class FungleTranslation
    {
        public static StringNames FailedToSync { get; } = new Translator("Failed to sync settings with host.\nError: ")
                    .AddTranslation(SupportedLangs.Brazilian, "Houve um erro ao sincronizar as configurações com o anfitrião\nErro: ");
        public static StringNames GameModeText { get; } = new Translator("Game mode")
                    .AddTranslation(SupportedLangs.Brazilian, "Modo de jogo");
        public static StringNames VictoryText { get; } = new Translator("Victory of the")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória do");
        public static StringNames ChanceText { get; } = new Translator("Chance")
                    .AddTranslation(SupportedLangs.Brazilian, "Chance");
        public static StringNames PriorityText { get; } = new Translator("Priority")
                    .AddTranslation(SupportedLangs.Brazilian, "Prioridade");
        public static StringNames FailedToSyncSettings { get; } = new Translator("There was a failure while trying to sync the settings.")
                    .AddTranslation(SupportedLangs.Brazilian, "Houve uma falha tentando sincronizar as configurações.");
        public static StringNames ChangeToPublicText { get; } = new Translator("Public rooms are disabled for security reasons.")
                    .AddTranslation(SupportedLangs.Brazilian, "Salas públicas estão desativadas por segurança.");
        public static StringNames LoadingPrefabsText { get; } = new Translator("Loading Prefabs")
                    .AddTranslation(SupportedLangs.Brazilian, "Carregando Predefinições");
        public static StringNames YourRoleIsText { get; } = new Translator("Your role is ")
                    .AddTranslation(SupportedLangs.Brazilian, "Sua função é ");
        public static StringNames EditText { get; } = new Translator("Edit preset")
                   .AddTranslation(SupportedLangs.Brazilian, "Editar predefinição");
        public static StringNames CountText { get; } = new Translator("Player Count")
                    .AddTranslation(SupportedLangs.Brazilian, "Quantidade de Jogadores");
        public static StringNames TeamPriorityText { get; } = new Translator("Team Priority")
                    .AddTranslation(SupportedLangs.Brazilian, "Prioridade do Time");
        public static StringNames ImpostorsText { get; } = new Translator("Impostors")
                    .AddTranslation(SupportedLangs.Brazilian, "Impostores");
        public static StringNames NeutralText { get; } = new Translator("Neutral")
                    .AddTranslation(SupportedLangs.Brazilian, "Neutro");
        public static StringNames NeutralsText { get; } = new Translator("Neutrals")
                    .AddTranslation(SupportedLangs.Brazilian, "Neutros");
        public static StringNames TeamsRemainText { get; } = new Translator("Remaining Teams: ")
                    .AddTranslation(SupportedLangs.Brazilian, "Times Restantes: ");
        public static StringNames NeutralGameOver { get; } = new Translator("Victory of the Neutrals")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Neutros");
        public static StringNames ImpostorGameOver { get; } = new Translator("Victory of the Impostors")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Impostores");
        public static StringNames CrewmateGameOver { get; } = new Translator("Crew Victory")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Tripulantes");
        public static StringNames TeamsText { get; } = new Translator("Teams")
                    .AddTranslation(SupportedLangs.Brazilian, "Times");
        public static StringNames TeamConfigButtonText { get; } = new Translator("Teams Settings")
                    .AddTranslation(SupportedLangs.Brazilian, "Configurações dos Times");
        public static StringNames TeamConfigDescText { get; } = new Translator("Edit the Teams settings for your lobby.")
                    .AddTranslation(SupportedLangs.Brazilian, "Edite as configurações de Times para sua sala.");
        public static StringNames TypeHereText { get; } = new Translator("Type here who was the killer")
                    .AddTranslation(SupportedLangs.Brazilian, "Digite aqui quem foi o assassino");
        public static class HandShakeFail
        {
            public static StringNames MissingAPIDisconnect { get; } = new Translator("{0} does not have the FungleAPI.")
                    .AddTranslation(SupportedLangs.Brazilian, "{0} não tem a FungleAPI.");
            public static StringNames MissingMods { get; } = new Translator("The following mods are missing from the local client:")
                    .AddTranslation(SupportedLangs.Brazilian, "Faltam os seguintes mods no cliente local:");
            public static StringNames ExtraMods { get; } = new Translator("The following mods are missing from the host client:")
                    .AddTranslation(SupportedLangs.Brazilian, "Faltam os seguintes mods no cliente do anfitrião:");
            public static StringNames HostNotModded { get; } = new Translator("Host is not modded")
                    .AddTranslation(SupportedLangs.Brazilian, "O Anfitrião não está modificado");
        }
    }
}
