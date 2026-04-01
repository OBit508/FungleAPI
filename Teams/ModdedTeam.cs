using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Attributes;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.GameOver;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Il2CppSystem.Linq.Expressions;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Teams
{
    /// <summary>
    /// Base class to create a team
    /// </summary>
    [FungleIgnore]
    public abstract class ModdedTeam
    {
        public TeamOptions TeamOptions = new TeamOptions();
        public FloatGameSetting CountData;
        public FloatGameSetting PriorityData;
        public bool Initialized;
        /// <summary>
        /// Team id
        /// </summary>
        public int TeamId;
        /// <summary>
        /// Team color
        /// </summary>
        public abstract Color TeamColor { get; }
        /// <summary>
        /// Team name
        /// </summary>
        public abstract StringNames TeamName { get; }
        /// <summary>
        /// Plural team name
        /// </summary>
        public abstract StringNames PluralName { get; }
        /// <summary>
        /// Victory screen text
        /// </summary>
        public abstract string VictoryText { get; }
        /// <summary>
        /// Default game over
        /// </summary>
        public abstract CustomGameOver DefaultGameOver { get; }
        /// <summary>
        /// Default role assigned to this team
        /// </summary>
        public virtual RoleTypes DefaultRole { get; }
        /// <summary>
        /// Determines whether friendly fire is enabled
        /// </summary>
        public virtual bool FriendlyFire => true;
        /// <summary>
        /// Determines whether members know each other
        /// </summary>
        public virtual bool KnowMembers => false;
        /// <summary>
        /// Maximum number of players allowed in this team
        /// </summary>
        public virtual uint MaxCount => 3;
        /// <summary>
        /// Default number of players in this team
        /// </summary>
        public virtual uint DefaultCount => 1;
        /// <summary>
        /// Default priority value for team assignment
        /// </summary>
        public virtual uint DefaultPriority => 1;
        /// <summary>
        /// Determines whether only enabled roles can be assigned
        /// </summary>
        public virtual bool AssignOnlyEnabledRoles => true;
        public virtual void Initialize(ModPlugin plugin)
        {
            if (!Initialized)
            {
                TeamOptions.ExtraOptions = ConfigurationManager.RegisterAllOptions(GetType(), ConfigFileType.TeamOptions, plugin);
                Initialized = true;
            }
        }
        /// <summary>
        /// Creates the category header
        /// </summary>
        public virtual CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.None, 20);
            categoryHeaderEditRole.Background.color = TeamColor.Lighten(0.7f);
            categoryHeaderEditRole.countLabel.color = TeamColor;
            categoryHeaderEditRole.chanceLabel.color = TeamColor;
            categoryHeaderEditRole.blankLabel.color = TeamColor.Darken(0.7f);
            categoryHeaderEditRole.Title.text = PluralName.GetString();
            categoryHeaderEditRole.Title.color = TeamColor.Lighten(0.9f);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        /// <summary>
        /// Creates the category header
        /// </summary>
        public virtual CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(PrefabUtils.FindPrefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.CrewmateRolesHeader, 61);
            string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
            categoryHeaderRoleVariant.Title.text = names[0] + " " + names[1] + " " + TeamName.GetString();
            categoryHeaderRoleVariant.Background.color = TeamColor.Lighten();
            categoryHeaderRoleVariant.Title.color = categoryHeaderRoleVariant.Background.color.Darken();
            categoryHeaderRoleVariant.Title.enabled = true;
            for (int i = 2; i < categoryHeaderRoleVariant.transform.GetChildCount(); i++)
            {
                categoryHeaderRoleVariant.transform.GetChild(i).gameObject.SetActive(false);
            }
            return categoryHeaderRoleVariant;
        }
        /// <summary>
        /// Creates the team count option
        /// </summary>
        public virtual OptionBehaviour CreateCountOption(Transform transform)
        {
            NumberOption option = ModdedNumberOption.CreateNumberOption(transform, CountData, delegate (NumberOption option)
            {
                if (this == ModdedTeamManager.Impostors)
                {
                    GameOptionsManager.Instance.currentGameOptions.SetInt(Int32OptionNames.NumImpostors, (int)option.Value);
                }
                TeamOptions.SetCount((int)option.Value);
            });
            option.Value = TeamOptions.GetCount();
            return option;
        }
        /// <summary>
        /// Creates the team priority option
        /// </summary>
        public virtual OptionBehaviour CreatePriorityOption(Transform transform)
        {
            NumberOption option = ModdedNumberOption.CreateNumberOption(transform, PriorityData, delegate (NumberOption option)
            {
                TeamOptions.SetPriority((int)option.Value);
            });
            option.Value = TeamOptions.GetPriority();
            return option;
        }
    }
}