using FungleAPI.Api;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
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
        public override string VictoryText => FungleTranslation.CrewmateGameOver.GetString();
        public override void CallGameOver(GameManager gameManager) => gameManager.RpcEndGame<CrewmatesByTask>();
        public override byte DefaultCount => byte.MaxValue;
        public override bool HideInLobby => true;
        public override void BeginIntro(IntroCutscene introCutscene)
        {
            base.BeginIntro(introCutscene);
            introCutscene.ImpostorText.text = IntroHelpers.GetTeamsTextAsCrewmate();
        }
        public override CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.CrewmateRolesHeader, 20);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public override CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderRoleVariant>(), parent);
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
