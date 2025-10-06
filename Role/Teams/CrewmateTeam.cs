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
    public class CrewmateTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Palette.CrewmateBlue;
        public override StringNames TeamName => StringNames.Crewmate;
        public override StringNames PluralName => StringNames.Crewmates;
        public override List<GameOverReason> WinReason { get; } = new List<GameOverReason>() { GameOverReason.CrewmatesByVote, GameOverReason.CrewmatesByTask, GameOverReason.CrewmateDisconnect, GameOverReason.HideAndSeek_CrewmatesByTimer };
        public override CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate<CategoryHeaderEditRole>(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public override CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.CrewmateRolesHeader, 61);
            return categoryHeaderRoleVariant;
        }
    }
}
