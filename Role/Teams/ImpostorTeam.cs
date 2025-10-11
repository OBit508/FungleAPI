using AmongUs.GameOptions;
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

namespace FungleAPI.Role.Teams
{
    public class ImpostorTeam : ModdedTeam
    {
        public override bool FriendlyFire => false;
        public override bool KnowMembers => true;
        public override Color TeamColor => Palette.ImpostorRed;
        public override StringNames TeamName => StringNames.Impostor;
        public override StringNames PluralName { get; } = FungleTranslation.ImpostorsText;
        public override CustomGameOver DefaultGameOver => GameOverManager.Instance<ImpostorsByKill>();
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
            categoryHeaderRoleVariant.Title.enabled = true;
            return categoryHeaderRoleVariant;
        }
        public override RoleTypes DefaultRole => RoleTypes.Impostor;
        public override uint DefaultPriority => 10;
        public override bool AssignOnlyEnabledRoles => false;
        
    }
}
