using BepInEx.Configuration;
using FungleAPI.GameOver;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using Il2CppSystem.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Role.Teams
{
    public class ModdedTeam
    {
        public static ModdedTeam Crewmates => Instance<CrewmateTeam>();
        public static ModdedTeam Impostors => Instance<ImpostorTeam>();
        public static ModdedTeam Neutrals => Instance<NeutralTeam>();
        internal static object RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            Teams.Add(team);
            return team;
        }
        public static T Instance<T>() where T : ModdedTeam
        {
            foreach (ModdedTeam team in Teams)
            {
                if (team.GetType() == typeof(T))
                {
                    return team.SimpleCast<T>();
                }
            }
            return null;
        }
        public ModPlugin TeamPlugin => ModPlugin.GetModPlugin(GetType().Assembly);
        public virtual Color TeamColor => Palette.CrewmateBlue;
        public virtual StringNames TeamName => StringNames.None;
        public virtual StringNames PluralName => StringNames.None;
        public virtual bool FriendlyFire => true;
        public virtual bool KnowMembers => false;
        public virtual CustomGameOver DefaultGameOver { get; }
        public virtual string VictoryText { get; }
        public virtual CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.None, 20);
            categoryHeaderEditRole.Background.color = Helpers.Light(TeamColor, 0.7f);
            categoryHeaderEditRole.countLabel.color = TeamColor;
            categoryHeaderEditRole.chanceLabel.color = TeamColor;
            categoryHeaderEditRole.blankLabel.color = Helpers.Dark(TeamColor, 0.7f);
            categoryHeaderEditRole.Title.text = PluralName.GetString();
            categoryHeaderEditRole.Title.color = Helpers.Light(TeamColor, 0.9f);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        public virtual CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.CrewmateRolesHeader, 61);
            string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
            categoryHeaderRoleVariant.Title.text = names[0] + " " + names[1] + " " + TeamName.GetString();
            categoryHeaderRoleVariant.Background.color = Helpers.Light(TeamColor);
            categoryHeaderRoleVariant.Title.color = Helpers.Dark(categoryHeaderRoleVariant.Background.color);
            categoryHeaderRoleVariant.Title.enabled = true;
            return categoryHeaderRoleVariant;
        }
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
    }
}
