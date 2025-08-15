using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Translation;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class NeutralTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Color.gray;
        public override StringNames TeamName => Translator.GetOrCreate("Neutral").AddTranslation(SupportedLangs.Brazilian, "Neutro").StringName;
        public override StringNames PluralName => Translator.GetOrCreate("Neutrals").AddTranslation(SupportedLangs.Brazilian, "Neutros").StringName;
    }
}
