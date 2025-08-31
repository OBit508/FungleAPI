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

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedToggleOption : ModdedOption
    {
        public ModdedToggleOption(string configName)
            : base(configName) { }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(bool))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                bool value = (bool)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.GetString(), value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            ToggleOption option = GameObject.Instantiate(PrefabUtils.Prefab<ToggleOption>(), transform);
            option.enabled = false;
            bool value = bool.Parse(localValue.Value);
            SetUpFromData(option);
            option.TitleText.enabled = false;
            option.TitleText.text = ConfigName.GetString();
            option.TitleText.enabled = true;
            option.transform.GetChild(1).GetComponent<PassiveButton>().SetNewAction(delegate
            {
                value = !value;
                SetValue(value.ToString());
                option.CheckMark.gameObject.SetActive(value);
            });
            option.CheckMark.gameObject.SetActive(value);
            option.gameObject.SetActive(true);
            return option;
        }
    }
}
