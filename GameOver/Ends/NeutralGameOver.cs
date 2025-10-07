using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver.Ends
{
    public class NeutralGameOver : CustomGameOver
    {
        public static Translator neutralText;
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
        public override GameOverReason Reason { get; } = GameOverManager.GetValidGameOver();
        public override string WinText => neutralGameOver.GetString();
        public override Color BackgroundColor => Color.gray;
        public override Color NameColor => Color.gray;
        public override List<NetworkedPlayerInfo> GetWinners()
        {
            List<NetworkedPlayerInfo> winners = new List<NetworkedPlayerInfo>();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason) && !networkedPlayerInfo.IsDead)
                {
                    winners.Add(networkedPlayerInfo);
                }
            }
            return winners;
        }
    }
}
