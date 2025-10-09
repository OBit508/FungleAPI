using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.GameOver;
using FungleAPI.Role;
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

namespace FungleAPI.Role.Teams
{
    public class ModdedTeam
    {
        public static Translator count;
        public static Translator priority;
        public static StringNames CountText
        {
            get
            {
                if (count == null)
                {
                    count = new Translator("Player Count");
                    count.AddTranslation(SupportedLangs.Latam, "Cantidad de jugadores");
                    count.AddTranslation(SupportedLangs.Brazilian, "Quantidade de Jogadores");
                    count.AddTranslation(SupportedLangs.Portuguese, "Quantidade de Jogadores");
                    count.AddTranslation(SupportedLangs.Korean, "플레이어 수");
                    count.AddTranslation(SupportedLangs.Russian, "Количество игроков");
                    count.AddTranslation(SupportedLangs.Dutch, "Aantal spelers");
                    count.AddTranslation(SupportedLangs.Filipino, "Bilang ng mga manlalaro");
                    count.AddTranslation(SupportedLangs.French, "Nombre de joueurs");
                    count.AddTranslation(SupportedLangs.German, "Spieleranzahl");
                    count.AddTranslation(SupportedLangs.Italian, "Numero di giocatori");
                    count.AddTranslation(SupportedLangs.Japanese, "プレイヤー数");
                    count.AddTranslation(SupportedLangs.Spanish, "Cantidad de jugadores");
                    count.AddTranslation(SupportedLangs.SChinese, "玩家数量");
                    count.AddTranslation(SupportedLangs.TChinese, "玩家數量");
                    count.AddTranslation(SupportedLangs.Irish, "Líon na n-imreoirí");
                }
                return count.StringName;
            }
        }
        public static StringNames PriorityText
        {
            get
            {
                if (priority == null)
                {
                    priority = new Translator("Team Priority");
                    priority.AddTranslation(SupportedLangs.Latam, "Prioridad del equipo");
                    priority.AddTranslation(SupportedLangs.Brazilian, "Prioridade do Time");
                    priority.AddTranslation(SupportedLangs.Portuguese, "Prioridade da Equipe");
                    priority.AddTranslation(SupportedLangs.Korean, "팀 우선순위");
                    priority.AddTranslation(SupportedLangs.Russian, "Приоритет команды");
                    priority.AddTranslation(SupportedLangs.Dutch, "Teamprioriteit");
                    priority.AddTranslation(SupportedLangs.Filipino, "Prayoridad ng Koponan");
                    priority.AddTranslation(SupportedLangs.French, "Priorité de l'équipe");
                    priority.AddTranslation(SupportedLangs.German, "Team-Priorität");
                    priority.AddTranslation(SupportedLangs.Italian, "Priorità della squadra");
                    priority.AddTranslation(SupportedLangs.Japanese, "チームの優先順位");
                    priority.AddTranslation(SupportedLangs.Spanish, "Prioridad del equipo");
                    priority.AddTranslation(SupportedLangs.SChinese, "队伍优先级");
                    priority.AddTranslation(SupportedLangs.TChinese, "隊伍優先級");
                    priority.AddTranslation(SupportedLangs.Irish, "Tosaíocht Foirne");
                }
                return priority.StringName;
            }
        }
        public static ModdedTeam Crewmates => Instance<CrewmateTeam>();
        public static ModdedTeam Impostors => Instance<ImpostorTeam>();
        public static ModdedTeam Neutrals => Instance<NeutralTeam>();
        internal static object RegisterTeam(Type type, ModPlugin plugin)
        {
            ModdedTeam team = (ModdedTeam)Activator.CreateInstance(type);
            plugin.Teams.Add(team);
            Teams.Add(team);
            ConfigurationManager.InitializeTeamCountAndPriority(team, plugin);
            team.CountData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.CountData.Type = OptionTypes.Float;
            team.CountData.Title = CountText;
            team.CountData.Increment = 1;
            team.CountData.ValidRange = new FloatRange(0, team.MaxCount);
            team.CountData.FormatString = null;
            team.CountData.ZeroIsInfinity = false;
            team.CountData.SuffixType = NumberSuffixes.None;
            team.CountData.OptionName = FloatOptionNames.Invalid;
            team.PriorityData = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            team.PriorityData.Type = OptionTypes.Float;
            team.PriorityData.Title = PriorityText;
            team.PriorityData.Increment = 1;
            team.PriorityData.ValidRange = new FloatRange(0, 500);
            team.PriorityData.FormatString = null;
            team.PriorityData.ZeroIsInfinity = false;
            team.PriorityData.SuffixType = NumberSuffixes.None;
            team.PriorityData.OptionName = FloatOptionNames.Invalid;
            plugin.BasePlugin.Log.LogInfo("Registered Team " + type.Name);
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
        public virtual void Initialize()
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
                        att.Initialize(type, property, this);
                        MethodInfo method = property.GetGetMethod(true);
                        if (method != null)
                        {
                            ConfigurationManager.Configs.Add(method, att);
                            FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(ConfigurationManager).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(MethodBase), property.PropertyType.MakeByRefType() }, null)));
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
        public static List<ModdedTeam> Teams = new List<ModdedTeam>();
    }
}
