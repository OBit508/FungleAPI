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
        public static StringNames ChanceText { get; } = new Translator("Chance")
                    .AddTranslation(SupportedLangs.Brazilian, "Chance").StringName;
        public static StringNames PriorityText { get; } = new Translator("Priority")
                    .AddTranslation(SupportedLangs.Brazilian, "Prioridade").StringName;
        public static StringNames FailedToSyncSettings { get; } = new Translator("There was a failure while trying to sync the settings.")
                    .AddTranslation(SupportedLangs.Brazilian, "Houve uma falha tentando sincronizar as configurações.").StringName;
        public static StringNames ChangeToPublicText { get; } = new Translator("Public rooms are disabled for security reasons.")
                    .AddTranslation(SupportedLangs.Brazilian, "Salas públicas estão desativadas por segurança.").StringName;
        public static StringNames LoadingPrefabsText { get; } = new Translator("Loading Prefabs")
                    .AddTranslation(SupportedLangs.Brazilian, "Carregando Predefinições").StringName;
        public static StringNames YourRoleIsText { get; } = new Translator("Your role is ")
                    .AddTranslation(SupportedLangs.Brazilian, "Sua função é ").StringName;
        public static StringNames EditText { get; } = new Translator("Edit preset")
                   .AddTranslation(SupportedLangs.Brazilian, "Editar predefinição").StringName;
        public static StringNames CountText { get; } = new Translator("Player Count")
                    .AddTranslation(SupportedLangs.Brazilian, "Quantidade de Jogadores").StringName;
        public static StringNames TeamPriorityText { get; } = new Translator("Team Priority")
                    .AddTranslation(SupportedLangs.Brazilian, "Prioridade do Time").StringName;
        public static StringNames ImpostorsText { get; } = new Translator("Impostors")
                    .AddTranslation(SupportedLangs.Brazilian, "Impostores").StringName;
        public static StringNames NeutralText { get; } = new Translator("Neutral")
                    .AddTranslation(SupportedLangs.Brazilian, "Neutro").StringName;
        public static StringNames NeutralsText { get; } = new Translator("Neutrals")
                    .AddTranslation(SupportedLangs.Brazilian, "Neutros").StringName;
        public static StringNames TeamsRemainText { get; } = new Translator("Remaining Teams: ")
                    .AddTranslation(SupportedLangs.Brazilian, "Times Restantes: ").StringName;
        public static StringNames NeutralGameOver { get; } = new Translator("Victory of the Neutrals")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Neutros").StringName;
        public static StringNames ImpostorGameOver { get; } = new Translator("Victory of the Impostors")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Impostores").StringName;
        public static StringNames CrewmateGameOver { get; } = new Translator("Crew Victory")
                    .AddTranslation(SupportedLangs.Brazilian, "Vitória dos Tripulantes").StringName;
        public static StringNames TeamsText { get; } = new Translator("Teams")
                    .AddTranslation(SupportedLangs.Brazilian, "Times").StringName;
        public static StringNames TeamConfigButtonText { get; } = new Translator("Teams Settings")
                    .AddTranslation(SupportedLangs.Brazilian, "Configurações dos Times").StringName;
        public static StringNames TeamConfigDescText { get; } = new Translator("Edit the Teams settings for your lobby.")
                    .AddTranslation(SupportedLangs.Brazilian, "Edite as configurações de Times para sua sala.").StringName;
        public static StringNames TypeHereText { get; } = new Translator("Type here who was the killer")
                    .AddTranslation(SupportedLangs.Brazilian, "Digite aqui quem foi o assassino").StringName;
    }
}
