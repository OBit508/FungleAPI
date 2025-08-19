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
    public class ImpostorTeam : ModdedTeam
    {
        public override bool FriendlyFire => false;
        public override bool KnowMembers => true;
        public override Color TeamColor => Palette.ImpostorRed;
        public override StringNames TeamName => StringNames.Impostor;
        public override StringNames PluralName { get; } = impostors;
        public override int MaxCount
        {
            get
            {
                try
                {
                    return GameOptionsManager.Instance.currentGameOptions.GetInt(AmongUs.GameOptions.Int32OptionNames.NumImpostors);
                }
                catch
                {
                    return 0;
                }
            }
        }
        internal static StringNames impostors
        {
            get
            {
                Translator translator = new Translator("Impostors");
                translator.AddTranslation(SupportedLangs.Latam, "Impostores");
                translator.AddTranslation(SupportedLangs.Brazilian, "Impostores");
                translator.AddTranslation(SupportedLangs.Portuguese, "Impostores");
                translator.AddTranslation(SupportedLangs.Korean, "임포스터들");
                translator.AddTranslation(SupportedLangs.Russian, "Самозванцы");
                translator.AddTranslation(SupportedLangs.Dutch, "Bedriegers");
                translator.AddTranslation(SupportedLangs.Filipino, "Mga Manlilinlang");
                translator.AddTranslation(SupportedLangs.French, "Imposteurs");
                translator.AddTranslation(SupportedLangs.German, "Betrüger");
                translator.AddTranslation(SupportedLangs.Italian, "Imbroglioni");
                translator.AddTranslation(SupportedLangs.Japanese, "インポスター");
                translator.AddTranslation(SupportedLangs.Spanish, "Impostores");
                translator.AddTranslation(SupportedLangs.SChinese, "内鬼");
                translator.AddTranslation(SupportedLangs.TChinese, "内鬼");
                translator.AddTranslation(SupportedLangs.Irish, "Bréagadóirí");
                return translator.StringName;
            }
        }
    }
}
