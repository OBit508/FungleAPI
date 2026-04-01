using AmongUs.GameOptions;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using xCloud;
using static PlayerMaterial;

namespace FungleAPI.Configuration.Attributes
{
    /// <summary>
    /// Attribute used in properties to transform them into number settings
    /// </summary>
    [HarmonyPatch(typeof(NumberOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedNumberOption : ModdedOption
    {
        public ModdedNumberOption(string configName, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
            : base(configName)
        {
            Data = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            FloatGameSetting floatGameSetting = (FloatGameSetting)Data;
            floatGameSetting.Type = OptionTypes.Float;
            floatGameSetting.Increment = increment;
            floatGameSetting.ValidRange = new FloatRange(minValue, maxValue);
            floatGameSetting.FormatString = formatString;
            floatGameSetting.ZeroIsInfinity = zeroIsInfinity;
            floatGameSetting.SuffixType = suffixType;
            floatGameSetting.OptionName = FloatOptionNames.Invalid;
        }
        public override void Initialize(PropertyInfo property, ModPlugin modPlugin)
        {
            base.Initialize(property, modPlugin);
            if (IsValidType(property.PropertyType))
            {
                float value = float.Parse(property.GetValue(null).ToString());
                localValue = FileManager.GetConfigFile(modPlugin, OptionType).Bind(property.DeclaringType.Name, property.Name, value.ToString());
                onlineValue = value.ToString();
                Data.SafeCast<FloatGameSetting>().Value = float.Parse(localValue.Value);
            }
        }
        public override object GetReturnedValue()
        {
            Type type = Property.PropertyType;
            if (type == typeof(int))
            {
                return int.Parse(GetValue());
            }
            else if (type == typeof(float))
            {
                return float.Parse(GetValue());
            }
            return GetValue();
        }
        public override bool IsValidType(Type type)
        {
            return type == typeof(float) || type == typeof(int);
        }
        public static NumberOption CreateNumberOption(Transform transform, FloatGameSetting data, Action<NumberOption> onChange)
        {
            NumberOption option = GameObject.Instantiate(PrefabUtils.FindPrefab<NumberOption>(), Vector3.zero, Quaternion.identity, transform);
            option.SetUpFromData(data, 20);
            option.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                onChange(option);
            });
            option.Title = data.Title;
            option.Increment = data.Increment;
            option.ValidRange = data.ValidRange;
            option.FormatString = data.FormatString;
            option.ZeroIsInfinity = data.ZeroIsInfinity;
            option.SuffixType = data.SuffixType;
            option.floatOptionName = FloatOptionNames.Invalid;
            FixOption(option);
            return option;
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            NumberOption option = CreateNumberOption(transform, setting, delegate (NumberOption option)
            {
                SetValue(option.Value);
                setting.Value = option.Value;
            });
            option.Value = float.Parse(localValue.Value);
            return option;
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(NumberOption __instance)
        {
            if (__instance.name == "ModdedOption")
            {
                __instance.AdjustButtonsActiveState();
                __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                NumberOption numberOption = __instance.data.SafeCast<NumberOption>();
                if (numberOption != null)
                {
                    __instance.Value = numberOption.Value;
                }
                return false;
            }
            return true;
        }
        [HarmonyPatch("UpdateValue")]
        [HarmonyPrefix]
        public static bool UpdateValuePrefix(NumberOption __instance)
        {
            return __instance.name != "ModdedOption";
        }
    }
}
