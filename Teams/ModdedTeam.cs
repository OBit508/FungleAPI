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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static Il2CppSystem.Globalization.CultureInfo;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace FungleAPI.Teams
{
    [FungleIgnore]
    public class ModdedTeam
    {
        public static ModdedTeam Crewmates => Instance<CrewmateTeam>();
        public static ModdedTeam Impostors => Instance<ImpostorTeam>();
        public static ModdedTeam Neutrals => Instance<NeutralTeam>();
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
        public virtual Color TeamColor => Palette.CrewmateBlue;
        public virtual StringNames TeamName => StringNames.None;
        public virtual StringNames PluralName => StringNames.None;
        public virtual bool FriendlyFire => true;
        public virtual bool KnowMembers => false;
        public virtual CustomGameOver DefaultGameOver { get; }
        public virtual string VictoryText { get; }
        public virtual uint MaxCount => 3;
        public virtual uint DefaultCount => 1;
        public virtual uint DefaultPriority => 1;
        public virtual RoleTypes DefaultRole { get; }
        public virtual bool AssignOnlyEnabledRoles => true;
        public virtual CategoryHeaderEditRole CreatCategoryHeaderEditRole(Transform parent)
        {
            CategoryHeaderEditRole categoryHeaderEditRole = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<CategoryHeaderEditRole>(), Vector3.zero, Quaternion.identity, parent);
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
        public virtual CategoryHeaderRoleVariant CreateCategoryHeaderRoleVariant(Transform parent)
        {
            CategoryHeaderRoleVariant categoryHeaderRoleVariant = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<CategoryHeaderRoleVariant>(), parent);
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
        public virtual void Initialize(ModPlugin plugin)
        {
            if (!initialized)
            {
                Type type = GetType();
                ExtraConfigs = new List<ModdedOption>();
                foreach (PropertyInfo property in type.GetProperties())
                {
                    ModdedOption att = (ModdedOption)property.GetCustomAttribute(typeof(ModdedOption));
                    if (att != null)
                    {
                        att.Initialize(property);
                        MethodInfo method = property.GetGetMethod(true);
                        if (method != null)
                        {
                            ConfigurationManager.Options.Add(att);
                            plugin.Options.Add(att);
                            HarmonyHelper.Patches.Add(method, new Func<object>(att.GetReturnedValue));
                            FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(HarmonyHelper).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public)));
                        }
                        ExtraConfigs.Add(att);
                    }
                }
                initialized = true;
            }
        }
        public virtual OptionBehaviour CreateCountOption(Transform transform)
        {
            NumberOption numberOption = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<NumberOption>(), Vector3.zero, Quaternion.identity, transform);
            numberOption.SetUpFromData(CountData, 20);
            numberOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                if (this == Impostors)
                {
                    GameOptionsManager.Instance.currentGameOptions.SetInt(Int32OptionNames.NumImpostors, (int)numberOption.Value);
                }
                CountAndPriority.SetCount((int)numberOption.Value);
            });
            numberOption.Title = CountData.Title;
            numberOption.Value = CountAndPriority.GetCount();
            numberOption.oldValue = numberOption.oldValue;
            numberOption.Increment = CountData.Increment;
            numberOption.ValidRange = CountData.ValidRange;
            numberOption.FormatString = CountData.FormatString;
            numberOption.ZeroIsInfinity = CountData.ZeroIsInfinity;
            numberOption.SuffixType = CountData.SuffixType;
            numberOption.floatOptionName = FloatOptionNames.Invalid;
            ModdedOption.FixOption(numberOption);
            return numberOption;
        }
        public virtual OptionBehaviour CreatePriorityOption(Transform transform)
        {
            NumberOption numberOption = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<NumberOption>(), Vector3.zero, Quaternion.identity, transform);
            numberOption.SetUpFromData(CountData, 20);
            numberOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                CountAndPriority.SetPriority((int)numberOption.Value);
            });
            numberOption.Title = PriorityData.Title;
            numberOption.Value = CountAndPriority.GetPriority();
            numberOption.oldValue = numberOption.oldValue;
            numberOption.Increment = PriorityData.Increment;
            numberOption.ValidRange = PriorityData.ValidRange;
            numberOption.FormatString = PriorityData.FormatString;
            numberOption.ZeroIsInfinity = PriorityData.ZeroIsInfinity;
            numberOption.SuffixType = PriorityData.SuffixType;
            numberOption.floatOptionName = FloatOptionNames.Invalid;
            ModdedOption.FixOption(numberOption);
            return numberOption;
        }
        public TeamCountAndPriority CountAndPriority;
        public FloatGameSetting CountData;
        public FloatGameSetting PriorityData;
        public List<ModdedOption> ExtraConfigs;
        public bool initialized;
        public int TeamId;
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
    }
}
