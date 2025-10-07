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
    public class ImpostorsByKill : CustomGameOver
    {
        public static Translator impostorText;
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
        public override string WinText => ImpostorGameOver.GetString();
        public override Color BackgroundColor { get; } = Color.red;
        public override Color NameColor { get; } = Color.red;
        public override GameOverReason Reason => GameOverReason.ImpostorsByKill;
    }
}
