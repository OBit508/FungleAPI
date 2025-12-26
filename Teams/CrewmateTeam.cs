using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
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
    public class CrewmateTeam : ModdedTeam
    {
        public override bool FriendlyFire => true;
        public override bool KnowMembers => false;
        public override Color TeamColor => Palette.CrewmateBlue;
        public override StringNames TeamName => StringNames.Crewmate;
        public override StringNames PluralName => StringNames.Crewmates;
        public override CustomGameOver DefaultGameOver => GameOverManager.Instance<CrewmatesByTask>();
        public override bool AssignOnlyEnabledRoles => false;
        public override uint DefaultCount => uint.MaxValue;
        public override CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public override CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.CrewmateRolesHeader, 61);
            categoryHeaderRoleVariant.Title.enabled = true;
            for (int i = 2; i < categoryHeaderRoleVariant.transform.GetChildCount(); i++)
            {
                categoryHeaderRoleVariant.transform.GetChild(i).gameObject.SetActive(false);
            }
            return categoryHeaderRoleVariant;
        }
    }
}
