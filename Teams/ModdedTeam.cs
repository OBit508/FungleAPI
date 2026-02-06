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
    /// 
    /// </summary>
    [FungleIgnore]
    public abstract class ModdedTeam
    {
        /// <summary>
        /// 
        /// </summary>
        public static ModdedTeam Crewmates => Instance<CrewmateTeam>();
        /// <summary>
        /// 
        /// </summary>
        public static ModdedTeam Impostors => Instance<ImpostorTeam>();
        /// <summary>
        /// 
        /// </summary>
        public static ModdedTeam Neutrals => Instance<NeutralTeam>();
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
        public TeamCountAndPriority CountAndPriority;
        public FloatGameSetting CountData;
        public FloatGameSetting PriorityData;
        public List<ModdedOption> ExtraConfigs;
        public bool Initialized;
        /// <summary>
        /// 
        /// </summary>
        public int TeamId;
        /// <summary>
        /// 
        /// </summary>
        public abstract Color TeamColor { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract StringNames TeamName { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract StringNames PluralName { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract string VictoryText { get; }
        /// <summary>
        /// 
        /// </summary>
        public abstract CustomGameOver DefaultGameOver { get; }
        /// <summary>
        /// 
        /// </summary>
        public virtual RoleTypes DefaultRole { get; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool FriendlyFire => true;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool KnowMembers => false;
        /// <summary>
        /// 
        /// </summary>
        public virtual uint MaxCount => 3;
        /// <summary>
        /// 
        /// </summary>
        public virtual uint DefaultCount => 1;
        /// <summary>
        /// 
        /// </summary>
        public virtual uint DefaultPriority => 1;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool AssignOnlyEnabledRoles => true;
        /// <summary>
        /// 
        /// </summary>
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
        public virtual void Initialize(ModPlugin plugin)
        {
            if (!Initialized)
            {
                ExtraConfigs = ConfigurationManager.RegisterAllOptions(GetType(), plugin);
                Initialized = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
            categoryHeaderEditRole.SetHeader(StringNames.None, 20);
            categoryHeaderEditRole.Background.color = TeamColor.Light(0.7f);
            categoryHeaderEditRole.countLabel.color = TeamColor;
            categoryHeaderEditRole.chanceLabel.color = TeamColor;
            categoryHeaderEditRole.blankLabel.color = TeamColor.Dark(0.7f);
            categoryHeaderEditRole.Title.text = PluralName.GetString();
            categoryHeaderEditRole.Title.color = TeamColor.Light(0.9f);
            categoryHeaderEditRole.Title.enabled = true;
            return categoryHeaderEditRole;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = GameObject.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
            categoryHeaderRoleVariant.SetHeader(StringNames.CrewmateRolesHeader, 61);
            string[] names = StringNames.CrewmateRolesHeader.GetString().Split(" ");
            categoryHeaderRoleVariant.Title.text = names[0] + " " + names[1] + " " + TeamName.GetString();
            categoryHeaderRoleVariant.Background.color = TeamColor.Light();
            categoryHeaderRoleVariant.Title.color = categoryHeaderRoleVariant.Background.color.Dark();
            categoryHeaderRoleVariant.Title.enabled = true;
            for (int i = 2; i < categoryHeaderRoleVariant.transform.GetChildCount(); i++)
            {
                categoryHeaderRoleVariant.transform.GetChild(i).gameObject.SetActive(false);
            }
            return categoryHeaderRoleVariant;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual OptionBehaviour CreateCountOption(Transform transform)
        {
            NumberOption option = ModdedNumberOption.CreateNumberOption(transform, CountData, delegate (NumberOption option)
            {
                if (this == Impostors)
                {
                    GameOptionsManager.Instance.currentGameOptions.SetInt(Int32OptionNames.NumImpostors, (int)option.Value);
                }
                CountAndPriority.SetCount((int)option.Value);
            });
            option.Value = CountAndPriority.GetCount();
            return option;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual OptionBehaviour CreatePriorityOption(Transform transform)
        {
            NumberOption option = ModdedNumberOption.CreateNumberOption(transform, PriorityData, delegate (NumberOption option)
            {
                CountAndPriority.SetPriority((int)option.Value);
            });
            option.Value = CountAndPriority.GetPriority();
            return option;
        }
    }
}
