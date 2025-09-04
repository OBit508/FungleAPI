using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
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

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedToggleOption : ModdedOption
    {
        public ModdedToggleOption(string configName)
            : base(configName) 
        {
            Data = ScriptableObject.CreateInstance<CheckboxGameSetting>();
            CheckboxGameSetting checkboxGameSetting = (CheckboxGameSetting)Data;
            checkboxGameSetting.Title = new Translator(ConfigName).StringName;
            checkboxGameSetting.Type = OptionTypes.Checkbox;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(bool))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                bool value = (bool)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            ToggleOption toggleOption = GameObject.Instantiate<ToggleOption>(PrefabUtils.Prefab<ToggleOption>(), Vector3.zero, Quaternion.identity, transform);
            toggleOption.SetUpFromData(Data, 20);
            toggleOption.Title = Data.Title;
            toggleOption.TitleText.text = Data.Title.GetString();
            toggleOption.CheckMark.enabled = bool.Parse(localValue.Value);
            toggleOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                SetValue(toggleOption.CheckMark.enabled);
            });
            FixOption(toggleOption);
            return toggleOption;
        }
    }
}
