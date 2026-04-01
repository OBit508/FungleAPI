using AmongUs.GameOptions;
using FungleAPI.Configuration.Attributes;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Teams
{
    public class ImpostorTeam : ModdedTeam
    {
        [ModdedNumberOption("Sla mano testand", 0, 50)]
        public static float SlaTeste => 10;
        public override bool FriendlyFire => false;
        public override bool KnowMembers => true;
        public override Color TeamColor => Palette.ImpostorRed;
        public override StringNames TeamName => StringNames.Impostor;
        public override StringNames PluralName { get; } = FungleTranslation.ImpostorsText;
        public override string VictoryText => FungleTranslation.ImpostorGameOver.GetString();
        public override CustomGameOver DefaultGameOver => GameOverManager.GetGameOverInstance<ImpostorsByKill>();
        public override CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.ImpostorRolesHeader, 20);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public override CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.ImpostorRolesHeader, 61);
            categoryHeaderRoleVariant.Title.enabled = true;
            for (int i = 2; i < categoryHeaderRoleVariant.transform.GetChildCount(); i++)
            {
                categoryHeaderRoleVariant.transform.GetChild(i).gameObject.SetActive(false);
            }
            return categoryHeaderRoleVariant;
        }
        public override RoleTypes DefaultRole => RoleTypes.Impostor;
        public override uint DefaultPriority => 10;
        public override bool AssignOnlyEnabledRoles => false;
        
    }
}
