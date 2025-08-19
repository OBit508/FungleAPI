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
    public class CustomNumberOption : CustomOption
    {
        public CustomNumberOption(string configName, float minValue, float maxValue, float reduceValue = 1, float increceValue = 1)
        {
            ReduceValue = reduceValue;
            IncreceValue = increceValue;
            ConfigName = configName;
            MaxValue = maxValue;
            MinValue = minValue;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(float))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                float value = (float)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            Option = GameObject.Instantiate<NumberOption>(Prefab<NumberOption>(), transform);
            float value = float.Parse(GetValue());
            SetUpFromData(Option);
            Option.TitleText.text = ConfigName;
            Option.ValueText.text = value.ToString();
            Option.MinusBtn.SetNewAction(delegate
            {
                if (value - ReduceValue >= 0 && value - ReduceValue >= MinValue)
                {
                    value -= ReduceValue;
                    SetValue(value.ToString());
                    Update(value);
                    Option.OnValueChanged?.Invoke(Option);
                }
            });
            Option.PlusBtn.SetNewAction(delegate
            {
                if (value + IncreceValue <= MaxValue)
                {
                    value += IncreceValue;
                    SetValue(value.ToString());
                    Update(value);
                    Option.OnValueChanged?.Invoke(Option);
                }
            });
            Update(value);
            Option.gameObject.SetActive(true);
            Option.enabled = false;
            return Option;
        }
        public void Update(float value)
        {
            Option.MinusBtn.SetInteractable(value > MinValue);
            Option.PlusBtn.SetInteractable(value < MaxValue);
            Option.ValueText.text = value.ToString();
        }
        public NumberOption Option;
        public float ReduceValue;
        public float IncreceValue;
        public float MinValue;
        public float MaxValue;
    }
}
