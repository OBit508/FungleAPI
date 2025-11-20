using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Components;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Rewired.UI.ControlMapper.ControlMapper;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace FungleAPI.Configuration.Attributes
{
    [HarmonyPatch(typeof(StringOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedEnumOption : ModdedOption
    {
        public ModdedEnumOption(string configName, string defaultValue)
            : base(configName)
        {
            Data = ScriptableObject.CreateInstance<StringGameSetting>().DontUnload();
            StringGameSetting stringGameSetting = (StringGameSetting)Data;
            stringGameSetting.Title = ConfigName.StringName;
            stringGameSetting.Type = OptionTypes.String;
            Values = defaultValue.Split("|");
            List<StringNames> stringNames = new List<StringNames>();
            foreach (string str in Values)
            {
                stringNames.Add(new Translator(str).StringName);
            }
            stringGameSetting.Values = stringNames.ToArray();
        }
        public override void Initialize(PropertyInfo property)
        {
            base.Initialize(property);
            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(string))
            {
                ModPlugin plugin = ModPluginManager.GetModPlugin(property.DeclaringType.Assembly);
                int value = property.PropertyType == typeof(int) ? (int)property.GetValue(null) : Values.GetIndex((string)property.GetValue(null));
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + property.DeclaringType.FullName, ConfigName.Default, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + property.DeclaringType.FullName + property.Name + value.GetType().FullName;
                Data.SafeCast<StringGameSetting>().Index = int.Parse(localValue.Value);
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            StringOption stringOption = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<StringOption>(), transform);
            StringGameSetting stringGameSetting = Data as StringGameSetting;
            stringOption.SetUpFromData(Data, 20);
            stringOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                stringGameSetting.Index = stringOption.Value;
            });
            stringOption.Title = stringGameSetting.Title;
            stringOption.Values = stringGameSetting.Values;
            stringOption.Value = stringGameSetting.Index;
            FixOption(stringOption);
            return stringOption;
        }
        public override object GetReturnedValue()
        {
            Type type = Property.PropertyType;
            if (Property.PropertyType == typeof(int) || Property.PropertyType == typeof(string))
            {
                return Property.PropertyType == typeof(int) ? int.Parse(GetValue()) : Values[int.Parse(GetValue())];
            }
            return GetValue();
        }
        public string[] Values;
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(StringOption __instance)
        {
            if (__instance.name == "ModdedOption")
            {
                __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                __instance.ValueText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Values[__instance.Value]);
                __instance.AdjustButtonsActiveState();
                return false;
            }
            return true;
        }
        [HarmonyPatch("UpdateValue")]
        [HarmonyPrefix]
        public static bool UpdateValuePrefix(StringOption __instance)
        {
            return __instance.name != "ModdedOption";
        }
    }
}
