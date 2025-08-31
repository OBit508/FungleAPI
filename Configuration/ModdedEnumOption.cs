using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
using FungleAPI.Networking;
using FungleAPI.Utilities;
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

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedEnumOption : ModdedOption
    {
        public ModdedEnumOption(string configName, string[] defaultValue)
            : base(configName)
        {
            Enum = defaultValue;
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(string))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                string value = (string)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName.GetString(), value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            StringOption option = UnityEngine.Object.Instantiate(PrefabUtils.Prefab<StringOption>(), transform);
            option.enabled = false;
            SetUpFromData(option);
            option.TitleText.enabled = false;
            option.TitleText.text = ConfigName.GetString();
            option.TitleText.enabled = true;
            option.ValueText.text = localValue.Value;
            option.MinusBtn.SetNewAction(delegate
            {
                BackValue();
                option.ValueText.text = localValue.Value;
                option.OnValueChanged?.Invoke(option);
            });
            option.PlusBtn.SetNewAction(delegate
            {
                NextValue();
                option.ValueText.text = localValue.Value;
                option.OnValueChanged?.Invoke(option);
            });
            option.Initialize();
            option.gameObject.SetActive(true);
            return option;
        }
        internal int currentIndex;
        internal string[] Enum;
        public void BackValue()
        {
            if (currentIndex - 1 <= 0)
            {
                currentIndex = Enum.Count() - 1;
            }
            else
            {
                currentIndex--;
            }
            SetValue(Enum[currentIndex]);
        }
        public void NextValue()
        {
            if (currentIndex + 1 >= Enum.Count())
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
            SetValue(Enum[currentIndex]);
        }
    }
}
