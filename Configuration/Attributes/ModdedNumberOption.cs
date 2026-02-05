using AmongUs.GameOptions;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using xCloud;

namespace FungleAPI.Configuration.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [HarmonyPatch(typeof(NumberOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedNumberOption : ModdedOption
    {
        /// <summary>
        /// 
        /// </summary>
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
                localValue = modPlugin.BasePlugin.Config.Bind(FullConfigName, ConfigName.Default, value.ToString());
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
        public override OptionBehaviour CreateOption(Transform transform)
        {
            NumberOption numberOption = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<NumberOption>(), Vector3.zero, Quaternion.identity, transform);
            FloatGameSetting floatGameSetting = (FloatGameSetting)Data;
            numberOption.SetUpFromData(Data, 20);
            numberOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                SetValue(numberOption.Value);
                floatGameSetting.Value = numberOption.Value;
            });
            numberOption.Title = floatGameSetting.Title;
            numberOption.Value = float.Parse(localValue.Value);
            numberOption.oldValue = numberOption.oldValue;
            numberOption.Increment = floatGameSetting.Increment;
            numberOption.ValidRange = floatGameSetting.ValidRange;
            numberOption.FormatString = floatGameSetting.FormatString;
            numberOption.ZeroIsInfinity = floatGameSetting.ZeroIsInfinity;
            numberOption.SuffixType = floatGameSetting.SuffixType;
            numberOption.floatOptionName = FloatOptionNames.Invalid;
            FixOption(numberOption);
            return numberOption;
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
