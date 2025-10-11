using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Translation
{
    public static class FungleTranslation
    {
        private static Translator count;
        private static Translator priority;
        private static Translator credits;
        private static Translator impostors;
        private static Translator neutral;
        private static Translator neutrals;
        private static Translator remainText;
        private static Translator neutralText;
        private static Translator impostorText;
        private static Translator crewmateText;
        private static Translator teams;
        private static Translator teamConfigButton;
        private static Translator teamConfigDesc;
        private static Translator nonModdedText;
        private static Translator kickingText;
        public static StringNames CreditsText
        {
            get
            {
                if (credits == null)
                {
                    credits = new Translator("Credits:");
                    credits.AddTranslation(SupportedLangs.Latam, "Créditos:");
                    credits.AddTranslation(SupportedLangs.Brazilian, "Créditos:");
                    credits.AddTranslation(SupportedLangs.Portuguese, "Créditos:");
                    credits.AddTranslation(SupportedLangs.Korean, "크레딧:");
                    credits.AddTranslation(SupportedLangs.Russian, "Авторы:");
                    credits.AddTranslation(SupportedLangs.Dutch, "Credits:");
                    credits.AddTranslation(SupportedLangs.Filipino, "Mga Kredito:");
                    credits.AddTranslation(SupportedLangs.French, "Crédits :");
                    credits.AddTranslation(SupportedLangs.German, "Credits:");
                    credits.AddTranslation(SupportedLangs.Italian, "Crediti:");
                    credits.AddTranslation(SupportedLangs.Japanese, "クレジット：");
                    credits.AddTranslation(SupportedLangs.Spanish, "Créditos:");
                    credits.AddTranslation(SupportedLangs.SChinese, "制作人员：");
                    credits.AddTranslation(SupportedLangs.TChinese, "製作人員：");
                    credits.AddTranslation(SupportedLangs.Irish, "Creidiúintí:");
                }
                return credits.StringName;
            }
        }
        public static StringNames CountText
        {
            get
            {
                if (count == null)
                {
                    count = new Translator("Player Count");
                    count.AddTranslation(SupportedLangs.Latam, "Cantidad de jugadores");
                    count.AddTranslation(SupportedLangs.Brazilian, "Quantidade de Jogadores");
                    count.AddTranslation(SupportedLangs.Portuguese, "Quantidade de Jogadores");
                    count.AddTranslation(SupportedLangs.Korean, "플레이어 수");
                    count.AddTranslation(SupportedLangs.Russian, "Количество игроков");
                    count.AddTranslation(SupportedLangs.Dutch, "Aantal spelers");
                    count.AddTranslation(SupportedLangs.Filipino, "Bilang ng mga manlalaro");
                    count.AddTranslation(SupportedLangs.French, "Nombre de joueurs");
                    count.AddTranslation(SupportedLangs.German, "Spieleranzahl");
                    count.AddTranslation(SupportedLangs.Italian, "Numero di giocatori");
                    count.AddTranslation(SupportedLangs.Japanese, "プレイヤー数");
                    count.AddTranslation(SupportedLangs.Spanish, "Cantidad de jugadores");
                    count.AddTranslation(SupportedLangs.SChinese, "玩家数量");
                    count.AddTranslation(SupportedLangs.TChinese, "玩家數量");
                    count.AddTranslation(SupportedLangs.Irish, "Líon na n-imreoirí");
                }
                return count.StringName;
            }
        }
        public static StringNames PriorityText
        {
            get
            {
                if (priority == null)
                {
                    priority = new Translator("Team Priority");
                    priority.AddTranslation(SupportedLangs.Latam, "Prioridad del equipo");
                    priority.AddTranslation(SupportedLangs.Brazilian, "Prioridade do Time");
                    priority.AddTranslation(SupportedLangs.Portuguese, "Prioridade da Equipe");
                    priority.AddTranslation(SupportedLangs.Korean, "팀 우선순위");
                    priority.AddTranslation(SupportedLangs.Russian, "Приоритет команды");
                    priority.AddTranslation(SupportedLangs.Dutch, "Teamprioriteit");
                    priority.AddTranslation(SupportedLangs.Filipino, "Prayoridad ng Koponan");
                    priority.AddTranslation(SupportedLangs.French, "Priorité de l'équipe");
                    priority.AddTranslation(SupportedLangs.German, "Team-Priorität");
                    priority.AddTranslation(SupportedLangs.Italian, "Priorità della squadra");
                    priority.AddTranslation(SupportedLangs.Japanese, "チームの優先順位");
                    priority.AddTranslation(SupportedLangs.Spanish, "Prioridad del equipo");
                    priority.AddTranslation(SupportedLangs.SChinese, "队伍优先级");
                    priority.AddTranslation(SupportedLangs.TChinese, "隊伍優先級");
                    priority.AddTranslation(SupportedLangs.Irish, "Tosaíocht Foirne");
                }
                return priority.StringName;
            }
        }
        public static StringNames ImpostorsText
        {
            get
            {
                if (impostors == null)
                {
                    impostors = new Translator("Impostors");
                    impostors.AddTranslation(SupportedLangs.Latam, "Impostores");
                    impostors.AddTranslation(SupportedLangs.Brazilian, "Impostores");
                    impostors.AddTranslation(SupportedLangs.Portuguese, "Impostores");
                    impostors.AddTranslation(SupportedLangs.Korean, "임포스터들");
                    impostors.AddTranslation(SupportedLangs.Russian, "Самозванцы");
                    impostors.AddTranslation(SupportedLangs.Dutch, "Bedriegers");
                    impostors.AddTranslation(SupportedLangs.Filipino, "Mga Manlilinlang");
                    impostors.AddTranslation(SupportedLangs.French, "Imposteurs");
                    impostors.AddTranslation(SupportedLangs.German, "Betrüger");
                    impostors.AddTranslation(SupportedLangs.Italian, "Imbroglioni");
                    impostors.AddTranslation(SupportedLangs.Japanese, "インポスター");
                    impostors.AddTranslation(SupportedLangs.Spanish, "Impostores");
                    impostors.AddTranslation(SupportedLangs.SChinese, "内鬼");
                    impostors.AddTranslation(SupportedLangs.TChinese, "内鬼");
                    impostors.AddTranslation(SupportedLangs.Irish, "Bréagadóirí");
                }
                return impostors.StringName;
            }
        }
        public static StringNames NeutralText
        {
            get
            {
                if (neutral == null)
                {
                    neutral = new Translator("Neutral");
                    neutral.AddTranslation(SupportedLangs.Latam, "Neutral");
                    neutral.AddTranslation(SupportedLangs.Brazilian, "Neutro");
                    neutral.AddTranslation(SupportedLangs.Portuguese, "Neutro");
                    neutral.AddTranslation(SupportedLangs.Korean, "중립자");
                    neutral.AddTranslation(SupportedLangs.Russian, "Нейтрал");
                    neutral.AddTranslation(SupportedLangs.Dutch, "Neutraal");
                    neutral.AddTranslation(SupportedLangs.Filipino, "Neutro");
                    neutral.AddTranslation(SupportedLangs.French, "Neutre");
                    neutral.AddTranslation(SupportedLangs.German, "Neutral");
                    neutral.AddTranslation(SupportedLangs.Italian, "Neutrale");
                    neutral.AddTranslation(SupportedLangs.Japanese, "ニュートラル");
                    neutral.AddTranslation(SupportedLangs.Spanish, "Neutral");
                    neutral.AddTranslation(SupportedLangs.SChinese, "中立");
                    neutral.AddTranslation(SupportedLangs.TChinese, "中立");
                    neutral.AddTranslation(SupportedLangs.Irish, "Neodrach");
                }
                return neutral.StringName;
            }
        }
        public static StringNames NeutralsText
        {
            get
            {
                if (neutrals == null)
                {
                    neutrals = new Translator("Neutrals");
                    neutrals.AddTranslation(SupportedLangs.Latam, "Neutrales");
                    neutrals.AddTranslation(SupportedLangs.Brazilian, "Neutros");
                    neutrals.AddTranslation(SupportedLangs.Portuguese, "Neutros");
                    neutrals.AddTranslation(SupportedLangs.Korean, "중립자들");
                    neutrals.AddTranslation(SupportedLangs.Russian, "Нейтралы");
                    neutrals.AddTranslation(SupportedLangs.Dutch, "Neutralen");
                    neutrals.AddTranslation(SupportedLangs.Filipino, "Mga Neutro");
                    neutrals.AddTranslation(SupportedLangs.French, "Neutres");
                    neutrals.AddTranslation(SupportedLangs.German, "Neutrale");
                    neutrals.AddTranslation(SupportedLangs.Italian, "Neutrali");
                    neutrals.AddTranslation(SupportedLangs.Japanese, "ニュートラル");
                    neutrals.AddTranslation(SupportedLangs.Spanish, "Neutrales");
                    neutrals.AddTranslation(SupportedLangs.SChinese, "中立者");
                    neutrals.AddTranslation(SupportedLangs.TChinese, "中立者");
                    neutrals.AddTranslation(SupportedLangs.Irish, "Neodracha");
                }
                return neutrals.StringName;
            }
        }
        public static StringNames TeamsRemainText
        {
            get
            {
                if (remainText == null)
                {
                    remainText = new Translator("Remaining Teams: ");
                    remainText.AddTranslation(SupportedLangs.Latam, "Equipos restantes: ");
                    remainText.AddTranslation(SupportedLangs.Brazilian, "Times Restantes: ");
                    remainText.AddTranslation(SupportedLangs.Portuguese, "Times Restantes: ");
                    remainText.AddTranslation(SupportedLangs.Korean, "남은 팀: ");
                    remainText.AddTranslation(SupportedLangs.Russian, "Оставшиеся команды: ");
                    remainText.AddTranslation(SupportedLangs.Dutch, "Resterende teams: ");
                    remainText.AddTranslation(SupportedLangs.Filipino, "Natitirang mga koponan: ");
                    remainText.AddTranslation(SupportedLangs.French, "Équipes restantes : ");
                    remainText.AddTranslation(SupportedLangs.German, "Verbleibende Teams: ");
                    remainText.AddTranslation(SupportedLangs.Italian, "Squadre rimanenti: ");
                    remainText.AddTranslation(SupportedLangs.Japanese, "残りのチーム: ");
                    remainText.AddTranslation(SupportedLangs.Spanish, "Equipos restantes: ");
                    remainText.AddTranslation(SupportedLangs.SChinese, "剩余队伍: ");
                    remainText.AddTranslation(SupportedLangs.TChinese, "剩餘隊伍: ");
                    remainText.AddTranslation(SupportedLangs.Irish, "Foirne atá fágtha: ");
                }
                return remainText.StringName;
            }
        }
        public static StringNames neutralGameOver
        {
            get
            {
                if (neutralText == null)
                {
                    neutralText = new Translator("Victory of the Neutrals");
                    neutralText.AddTranslation(SupportedLangs.Latam, "Victoria de los neutrales");
                    neutralText.AddTranslation(SupportedLangs.Brazilian, "Vitória dos Neutros");
                    neutralText.AddTranslation(SupportedLangs.Portuguese, "Vitória dos Neutros");
                    neutralText.AddTranslation(SupportedLangs.Korean, "중립의 승리");
                    neutralText.AddTranslation(SupportedLangs.Russian, "Победа нейтралов");
                    neutralText.AddTranslation(SupportedLangs.Dutch, "Overwinning van de neutralen");
                    neutralText.AddTranslation(SupportedLangs.Filipino, "Tagumpay ng mga neutral");
                    neutralText.AddTranslation(SupportedLangs.French, "Victoire des neutres");
                    neutralText.AddTranslation(SupportedLangs.German, "Sieg der Neutralen");
                    neutralText.AddTranslation(SupportedLangs.Italian, "Vittoria dei neutrali");
                    neutralText.AddTranslation(SupportedLangs.Japanese, "ニュートラルの勝利");
                    neutralText.AddTranslation(SupportedLangs.Spanish, "Victoria de los neutrales");
                    neutralText.AddTranslation(SupportedLangs.SChinese, "中立者的胜利");
                    neutralText.AddTranslation(SupportedLangs.TChinese, "中立者的勝利");
                    neutralText.AddTranslation(SupportedLangs.Irish, "Bua na Neodrach");
                }
                return neutralText.StringName;
            }
        }
        public static StringNames ImpostorGameOver
        {
            get
            {
                if (impostorText == null)
                {
                    impostorText = new Translator("Victory of the Impostors");
                    impostorText.AddTranslation(SupportedLangs.Latam, "Victoria de los impostores");
                    impostorText.AddTranslation(SupportedLangs.Brazilian, "Vitória dos Impostores");
                    impostorText.AddTranslation(SupportedLangs.Portuguese, "Vitória dos Impostores");
                    impostorText.AddTranslation(SupportedLangs.Korean, "임포스터의 승리");
                    impostorText.AddTranslation(SupportedLangs.Russian, "Победа предателей");
                    impostorText.AddTranslation(SupportedLangs.Dutch, "Overwinning van de bedriegers");
                    impostorText.AddTranslation(SupportedLangs.Filipino, "Tagumpay ng mga Impostor");
                    impostorText.AddTranslation(SupportedLangs.French, "Victoire des imposteurs");
                    impostorText.AddTranslation(SupportedLangs.German, "Sieg der Betrüger");
                    impostorText.AddTranslation(SupportedLangs.Italian, "Vittoria degli impostori");
                    impostorText.AddTranslation(SupportedLangs.Japanese, "インポスターの勝利");
                    impostorText.AddTranslation(SupportedLangs.Spanish, "Victoria de los impostores");
                    impostorText.AddTranslation(SupportedLangs.SChinese, "内鬼的胜利");
                    impostorText.AddTranslation(SupportedLangs.TChinese, "內鬼的勝利");
                    impostorText.AddTranslation(SupportedLangs.Irish, "Bua na bhFéintréitheoirí");
                }
                return impostorText.StringName;
            }
        }
        public static StringNames CrewmateGameOver
        {
            get
            {
                if (crewmateText == null)
                {
                    crewmateText = new Translator("Crew Victory");
                    crewmateText.AddTranslation(SupportedLangs.Latam, "Victoria de la tripulación");
                    crewmateText.AddTranslation(SupportedLangs.Brazilian, "Vitória dos Tripulantes");
                    crewmateText.AddTranslation(SupportedLangs.Portuguese, "Vitória dos Tripulantes");
                    crewmateText.AddTranslation(SupportedLangs.Korean, "승무원의 승리");
                    crewmateText.AddTranslation(SupportedLangs.Russian, "Победа команды");
                    crewmateText.AddTranslation(SupportedLangs.Dutch, "Overwinning van de bemanning");
                    crewmateText.AddTranslation(SupportedLangs.Filipino, "Tagumpay ng tripulante");
                    crewmateText.AddTranslation(SupportedLangs.French, "Victoire de l’équipage");
                    crewmateText.AddTranslation(SupportedLangs.German, "Sieg der Crew");
                    crewmateText.AddTranslation(SupportedLangs.Italian, "Vittoria dell’equipaggio");
                    crewmateText.AddTranslation(SupportedLangs.Japanese, "クルーの勝利");
                    crewmateText.AddTranslation(SupportedLangs.Spanish, "Victoria de la tripulación");
                    crewmateText.AddTranslation(SupportedLangs.SChinese, "船员的胜利");
                    crewmateText.AddTranslation(SupportedLangs.TChinese, "船員的勝利");
                    crewmateText.AddTranslation(SupportedLangs.Irish, "Bua na gCrúach");
                }
                return crewmateText.StringName;
            }
        }
        public static StringNames TeamsText
        {
            get
            {
                if (teams == null)
                {
                    teams = new Translator("Teams");
                    teams.AddTranslation(SupportedLangs.Latam, "Equipos");
                    teams.AddTranslation(SupportedLangs.Brazilian, "Times");
                    teams.AddTranslation(SupportedLangs.Portuguese, "Equipes");
                    teams.AddTranslation(SupportedLangs.Korean, "팀");
                    teams.AddTranslation(SupportedLangs.Russian, "Команды");
                    teams.AddTranslation(SupportedLangs.Dutch, "Teams");
                    teams.AddTranslation(SupportedLangs.Filipino, "Mga Koponan");
                    teams.AddTranslation(SupportedLangs.French, "Équipes");
                    teams.AddTranslation(SupportedLangs.German, "Teams");
                    teams.AddTranslation(SupportedLangs.Italian, "Squadre");
                    teams.AddTranslation(SupportedLangs.Japanese, "チーム");
                    teams.AddTranslation(SupportedLangs.Spanish, "Equipos");
                    teams.AddTranslation(SupportedLangs.SChinese, "队伍");
                    teams.AddTranslation(SupportedLangs.TChinese, "隊伍");
                    teams.AddTranslation(SupportedLangs.Irish, "Foirne");
                }
                return teams.StringName;
            }
        }
        public static StringNames TeamConfigButtonText
        {
            get
            {
                if (teamConfigButton == null)
                {
                    teamConfigButton = new Translator("Teams Configurations");
                    teamConfigButton.AddTranslation(SupportedLangs.Latam, "Configuraciones de equipos");
                    teamConfigButton.AddTranslation(SupportedLangs.Brazilian, "Configurações dos Times");
                    teamConfigButton.AddTranslation(SupportedLangs.Portuguese, "Configurações das Equipes");
                    teamConfigButton.AddTranslation(SupportedLangs.Korean, "팀 설정");
                    teamConfigButton.AddTranslation(SupportedLangs.Russian, "Настройки команд");
                    teamConfigButton.AddTranslation(SupportedLangs.Dutch, "Teamconfiguraties");
                    teamConfigButton.AddTranslation(SupportedLangs.Filipino, "Mga configuration ng koponan");
                    teamConfigButton.AddTranslation(SupportedLangs.French, "Configurations des équipes");
                    teamConfigButton.AddTranslation(SupportedLangs.German, "Teamkonfigurationen");
                    teamConfigButton.AddTranslation(SupportedLangs.Italian, "Configurazioni delle squadre");
                    teamConfigButton.AddTranslation(SupportedLangs.Japanese, "チーム設定");
                    teamConfigButton.AddTranslation(SupportedLangs.Spanish, "Configuraciones de equipos");
                    teamConfigButton.AddTranslation(SupportedLangs.SChinese, "队伍配置");
                    teamConfigButton.AddTranslation(SupportedLangs.TChinese, "隊伍配置");
                    teamConfigButton.AddTranslation(SupportedLangs.Irish, "Cumraíochtaí Foirne");
                }
                return teamConfigButton.StringName;
            }
        }
        public static StringNames TeamConfigDescText
        {
            get
            {
                if (teamConfigDesc == null)
                {
                    teamConfigDesc = new Translator("Edit the Teams settings for your lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Latam, "Edita la configuración de equipos para tu sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Brazilian, "Edite as configurações de Times para sua sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Portuguese, "Edite as configurações das Equipes para sua sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Korean, "로비의 팀 설정을 편집하세요.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Russian, "Измените настройки команд для вашей комнаты.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Dutch, "Bewerk de teaminstellingen voor je lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Filipino, "I-edit ang mga setting ng koponan para sa iyong lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.French, "Modifiez les paramètres des équipes pour votre salon.");
                    teamConfigDesc.AddTranslation(SupportedLangs.German, "Bearbeiten Sie die Team-Einstellungen für Ihre Lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Italian, "Modifica le impostazioni delle squadre per la tua lobby.");
                    teamConfigDesc.AddTranslation(SupportedLangs.Japanese, "ロビーのチーム設定を編集してください。");
                    teamConfigDesc.AddTranslation(SupportedLangs.Spanish, "Edita la configuración de equipos para tu sala.");
                    teamConfigDesc.AddTranslation(SupportedLangs.SChinese, "编辑您房间的队伍设置。");
                    teamConfigDesc.AddTranslation(SupportedLangs.TChinese, "編輯您房間的隊伍設定。");
                    teamConfigDesc.AddTranslation(SupportedLangs.Irish, "Cuir eagarthóireacht ar na socruithe foirne do do lóiste.");
                }
                return teamConfigDesc.StringName;
            }
        }
        public static StringNames NonModdedText
        {
            get
            {
                if (nonModdedText == null)
                {
                    nonModdedText = new Translator("Non Modded Clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Latam, "Clientes sin mods:");
                    nonModdedText.AddTranslation(SupportedLangs.Brazilian, "Clientes não modificados:");
                    nonModdedText.AddTranslation(SupportedLangs.Portuguese, "Clientes não modificados:");
                    nonModdedText.AddTranslation(SupportedLangs.Korean, "모드가 없는 클라이언트:");
                    nonModdedText.AddTranslation(SupportedLangs.Russian, "Клиенты без модов:");
                    nonModdedText.AddTranslation(SupportedLangs.Dutch, "Niet-gemodde clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Filipino, "Mga kliyenteng walang mods:");
                    nonModdedText.AddTranslation(SupportedLangs.French, "Clients sans mods :");
                    nonModdedText.AddTranslation(SupportedLangs.German, "Nicht-modifizierte Clients:");
                    nonModdedText.AddTranslation(SupportedLangs.Italian, "Client non moddati:");
                    nonModdedText.AddTranslation(SupportedLangs.Japanese, "非Modクライアント:");
                    nonModdedText.AddTranslation(SupportedLangs.Spanish, "Clientes sin mods:");
                    nonModdedText.AddTranslation(SupportedLangs.SChinese, "未安装模组的客户端:");
                    nonModdedText.AddTranslation(SupportedLangs.TChinese, "未安裝模組的客戶端:");
                    nonModdedText.AddTranslation(SupportedLangs.Irish, "Cliaint gan mods:");
                }
                return nonModdedText.StringName;
            }
        }
        public static StringNames KickingText
        {
            get
            {
                if (kickingText == null)
                {
                    kickingText = new Translator("Kicking in:");
                    kickingText.AddTranslation(SupportedLangs.Brazilian, "Expulsando em:");
                    kickingText.AddTranslation(SupportedLangs.Portuguese, "Expulsando em:");
                    kickingText.AddTranslation(SupportedLangs.Korean, "추방까지:");
                    kickingText.AddTranslation(SupportedLangs.Russian, "Исключение через:");
                    kickingText.AddTranslation(SupportedLangs.Dutch, "Uitzetten over:");
                    kickingText.AddTranslation(SupportedLangs.Filipino, "Pagtatanggal sa loob ng:");
                    kickingText.AddTranslation(SupportedLangs.French, "Expulsion dans :");
                    kickingText.AddTranslation(SupportedLangs.German, "Rauswurf in:");
                    kickingText.AddTranslation(SupportedLangs.Italian, "Espulsione tra:");
                    kickingText.AddTranslation(SupportedLangs.Japanese, "追放まで:");
                    kickingText.AddTranslation(SupportedLangs.Spanish, "Expulsando en:");
                    kickingText.AddTranslation(SupportedLangs.SChinese, "踢出倒计时:");
                    kickingText.AddTranslation(SupportedLangs.TChinese, "踢出倒數:");
                    kickingText.AddTranslation(SupportedLangs.Irish, "Ag ciceáil i:");

                }
                return kickingText.StringName;
            }
        }
    }
}
