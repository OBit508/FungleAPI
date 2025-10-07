using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class NeutralTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Color.gray;
        public override StringNames TeamName { get; } = neutral;
        public override StringNames PluralName { get; } = neutrals;
        public override CustomGameOver DefaultGameOver => GameOverManager.Instance<NeutralGameOver>();
        internal static StringNames neutral
        {
            get
            {
                Translator translator = new Translator("Neutral");
                translator.AddTranslation(SupportedLangs.Latam, "Neutral");
                translator.AddTranslation(SupportedLangs.Brazilian, "Neutro");
                translator.AddTranslation(SupportedLangs.Portuguese, "Neutro");
                translator.AddTranslation(SupportedLangs.Korean, "중립자"); 
                translator.AddTranslation(SupportedLangs.Russian, "Нейтрал"); 
                translator.AddTranslation(SupportedLangs.Dutch, "Neutraal");
                translator.AddTranslation(SupportedLangs.Filipino, "Neutro");
                translator.AddTranslation(SupportedLangs.French, "Neutre");
                translator.AddTranslation(SupportedLangs.German, "Neutral");
                translator.AddTranslation(SupportedLangs.Italian, "Neutrale");
                translator.AddTranslation(SupportedLangs.Japanese, "ニュートラル");
                translator.AddTranslation(SupportedLangs.Spanish, "Neutral");
                translator.AddTranslation(SupportedLangs.SChinese, "中立");
                translator.AddTranslation(SupportedLangs.TChinese, "中立");
                translator.AddTranslation(SupportedLangs.Irish, "Neodrach");
                return translator.StringName;
            }
        }
        internal static StringNames neutrals 
        {
            get
            {
                Translator translator = new Translator("Neutrals");
                translator.AddTranslation(SupportedLangs.Latam, "Neutrales");
                translator.AddTranslation(SupportedLangs.Brazilian, "Neutros");
                translator.AddTranslation(SupportedLangs.Portuguese, "Neutros");
                translator.AddTranslation(SupportedLangs.Korean, "중립자들");
                translator.AddTranslation(SupportedLangs.Russian, "Нейтралы");
                translator.AddTranslation(SupportedLangs.Dutch, "Neutralen");
                translator.AddTranslation(SupportedLangs.Filipino, "Mga Neutro");
                translator.AddTranslation(SupportedLangs.French, "Neutres");
                translator.AddTranslation(SupportedLangs.German, "Neutrale");
                translator.AddTranslation(SupportedLangs.Italian, "Neutrali");
                translator.AddTranslation(SupportedLangs.Japanese, "ニュートラル");
                translator.AddTranslation(SupportedLangs.Spanish, "Neutrales");
                translator.AddTranslation(SupportedLangs.SChinese, "中立者");
                translator.AddTranslation(SupportedLangs.TChinese, "中立者");
                translator.AddTranslation(SupportedLangs.Irish, "Neodracha");
                return translator.StringName;
            }
        }
    }
}
