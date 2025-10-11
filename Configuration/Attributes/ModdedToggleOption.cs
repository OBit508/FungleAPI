using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Components;
using FungleAPI.Patches;
using FungleAPI.Role;
using FungleAPI.Networking;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Rewired.UI.ControlMapper.ControlMapper;
using FungleAPI.Translation;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using FungleAPI.PluginLoading;

namespace FungleAPI.Configuration.Attributes
{
    [HarmonyPatch(typeof(ToggleOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedToggleOption : ModdedOption
    {
        public ModdedToggleOption(string configName, string groupId)
            : base(configName, groupId) 
        {
            Data = ScriptableObject.CreateInstance<CheckboxGameSetting>().DontUnload();
            CheckboxGameSetting checkboxGameSetting = (CheckboxGameSetting)Data;
            checkboxGameSetting.Title = ConfigName.StringName;
            checkboxGameSetting.Type = OptionTypes.Checkbox;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(bool))
            {
                ModPlugin plugin = ModPluginManager.GetModPlugin(type.Assembly);
                bool value = (bool)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.Default, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            ToggleOption toggleOption = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<ToggleOption>(), Vector3.zero, Quaternion.identity, transform);
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
