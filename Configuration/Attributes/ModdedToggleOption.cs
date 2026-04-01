using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using static PlayerMaterial;

namespace FungleAPI.Configuration.Attributes
{
    /// <summary>
    /// Attribute used in properties to transform them into boolean settings
    /// </summary>
    [HarmonyPatch(typeof(ToggleOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedToggleOption : ModdedOption
    {
        public ModdedToggleOption(string configName)
            : base(configName) 
        {
            Data = ScriptableObject.CreateInstance<CheckboxGameSetting>().DontUnload();
            CheckboxGameSetting checkboxGameSetting = (CheckboxGameSetting)Data;
            checkboxGameSetting.Type = OptionTypes.Checkbox;
        }
        public override void Initialize(PropertyInfo property, ModPlugin modPlugin)
        {
            base.Initialize(property, modPlugin);
            if (IsValidType(property.PropertyType))
            {
                ModPlugin plugin = ModPluginManager.GetModPlugin(property.DeclaringType.Assembly);
                bool value = property.PropertyType == typeof(bool) ? (bool)property.GetValue(null) : property.PropertyType == typeof(int) ? (int)property.GetValue(null) == 1 : (float)property.GetValue(null) == 1;
                localValue = FileManager.GetConfigFile(modPlugin, OptionType).Bind(property.DeclaringType.Name, property.Name, value.ToString());
                onlineValue = value.ToString();
            }
        }
        public override object GetReturnedValue()
        {
            Type type = Property.PropertyType;
            bool value = bool.Parse(GetValue());
            if (type == typeof(bool))
            {
                return value;
            }
            else if (type == typeof(int) || type == typeof(float))
            {
                return value ? 1 : 0;
            }
            return GetValue();
        }
        public override bool IsValidType(Type type)
        {
            return type == typeof(int) || type == typeof(float) || type == typeof(bool);
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            ToggleOption toggleOption = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<ToggleOption>(), Vector3.zero, Quaternion.identity, transform);
            toggleOption.SetUpFromData(Data, 20);
            toggleOption.Title = Data.Title;
            toggleOption.TitleText.text = Data.Title.GetString();
            toggleOption.oldValue = bool.Parse(localValue.Value);
            toggleOption.CheckMark.enabled = toggleOption.oldValue;
            toggleOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                SetValue(toggleOption.CheckMark.enabled);
            });
            FixOption(toggleOption);
            return toggleOption;
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(ToggleOption __instance)
        {
            if (__instance.name == "ModdedOption")
            {
                __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                return false;
            }
            return true;
        }
        [HarmonyPatch("UpdateValue")]
        [HarmonyPrefix]
        public static bool UpdateValuePrefix(ToggleOption __instance)
        {
            return __instance.name != "ModdedOption";
        }
    }
}
