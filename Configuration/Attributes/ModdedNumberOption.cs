using AmongUs.GameOptions;
using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using FungleAPI.Translation;
using HarmonyLib;
using FungleAPI.Utilities.Prefabs;

namespace FungleAPI.Configuration.Attributes
{
    [HarmonyPatch(typeof(NumberOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedNumberOption : ModdedOption
    {
        public ModdedNumberOption(string configName, string groupId, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
            : base(configName, groupId)
        {
            Data = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            FloatGameSetting floatGameSetting = (FloatGameSetting)Data;
            floatGameSetting.Type = OptionTypes.Float;
            floatGameSetting.Title = ConfigName.StringName;
            floatGameSetting.Increment = increment;
            floatGameSetting.ValidRange = new FloatRange(minValue, maxValue);
            floatGameSetting.FormatString = formatString;
            floatGameSetting.ZeroIsInfinity = zeroIsInfinity;
            floatGameSetting.SuffixType = suffixType;
            floatGameSetting.OptionName = FloatOptionNames.Invalid;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(float))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                float value = (float)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.Default, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
                Data.SafeCast<FloatGameSetting>().Value = float.Parse(localValue.Value);
            }
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
            numberOption.ValidRange = new FloatRange(floatGameSetting.ValidRange.min, floatGameSetting.ValidRange.max);
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
