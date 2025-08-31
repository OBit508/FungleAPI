using AmongUs.GameOptions;
using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Patches;
using FungleAPI.Roles;
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

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedNumberOption : ModdedOption
    {
        public ModdedNumberOption(string configName, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
            : base(configName)
        {
            Increment = increment;
            MaxValue = maxValue;
            MinValue = minValue;
            FormatString = formatString;
            ZeroIsInfinity = zeroIsInfinity;
            SuffixType = suffixType;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(float))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                float value = (float)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.GetString(), value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            Option = GameObject.Instantiate<NumberOption>(PrefabUtils.Prefab<NumberOption>(), transform);
            SetUpFromData(Option);
            Option.Title = ConfigName;
            Option.Value = float.Parse(GetValue());
            Option.Increment = Increment;
            Option.ValidRange = new FloatRange(MinValue, MaxValue);
            Option.FormatString = FormatString != null ? FormatString : "0";
            Option.ZeroIsInfinity = ZeroIsInfinity;
            Option.SuffixType = SuffixType;
            Option.floatOptionName = FloatOptionNames.Invalid;
            Option.ValueText.text = GetValue();
            Option.Initialize();
            Option.gameObject.SetActive(true);
            return Option;
        }
        public NumberOption Option;
        public float Increment;
        public string FormatString;
        public bool ZeroIsInfinity;
        public NumberSuffixes SuffixType;
        public float MinValue;
        public float MaxValue;
    }
}
