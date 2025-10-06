using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
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
        public override List<GameOverReason> WinReason { get; } = new List<GameOverReason>() { GameOverReason.ImpostorsByVote, GameOverReason.ImpostorsByKill, GameOverReason.ImpostorsBySabotage, GameOverReason.ImpostorDisconnect, GameOverReason.HideAndSeek_ImpostorsByKills };
        public override CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate<CategoryHeaderEditRole>(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.ImpostorRolesHeader, 20);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public override CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.ImpostorRolesHeader, 61);
            return categoryHeaderRoleVariant;
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
