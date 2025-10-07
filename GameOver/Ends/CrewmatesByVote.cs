using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver.Ends
{
    public class CrewmatesByVote : CustomGameOver
    {
        public static Translator crewmateText;
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
        public override string WinText => CrewmateGameOver.GetString();
        public override Color BackgroundColor { get; } = Palette.CrewmateBlue;
        public override Color NameColor { get; } = Palette.CrewmateBlue;
        public override GameOverReason Reason => GameOverReason.CrewmatesByVote;
    }
}
