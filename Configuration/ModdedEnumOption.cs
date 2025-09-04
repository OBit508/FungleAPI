using BepInEx.Configuration;
using Epic.OnlineServices;
using Epic.OnlineServices.RTC;
using FungleAPI.Components;
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
using FungleAPI.Translation;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;

namespace FungleAPI.Configuration
{
    [HarmonyPatch(typeof(StringOption))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedEnumOption : ModdedOption
    {
        public ModdedEnumOption(string configName, string[] defaultValue)
            : base(configName)
        {
            Data = ScriptableObject.CreateInstance<StringGameSetting>().DontUnload();
            StringGameSetting stringGameSetting = (StringGameSetting)Data;
            stringGameSetting.Title = new Translator(configName).StringName;
            stringGameSetting.Type = OptionTypes.String;
            List<StringNames> stringNames = new List<StringNames>();
            foreach (string str in defaultValue)
            {
                stringNames.Add(new Translator(str).StringName);
            }
            stringGameSetting.Values = stringNames.ToArray();
        }
        public override void Initialize(Type type, PropertyInfo property, object obj)
        {
            if (property.PropertyType == typeof(int))
            {
                ModPlugin plugin = ModPlugin.GetModPlugin(type.Assembly);
                int value = (int)property.GetValue(obj);
                localValue = plugin.BasePlugin.Config.Bind(plugin.ModName + " - " + type.FullName, ConfigName, value.ToString());
                onlineValue = value.ToString();
                FullConfigName = plugin.ModName + type.FullName + property.Name + value.GetType().FullName;
                Data.SafeCast<StringGameSetting>().Index = int.Parse(localValue.Value);
            }
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            StringOption stringOption = GameObject.Instantiate<StringOption>(PrefabUtils.Prefab<StringOption>(), transform);
            StringGameSetting stringGameSetting = Data as StringGameSetting;
            stringOption.SetUpFromData(Data, 20);
            stringOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                stringGameSetting.Index = stringOption.Value;
            });
            stringOption.Title = stringGameSetting.Title;
            stringOption.Values = stringGameSetting.Values;
            stringOption.Value = stringGameSetting.Index;
            return stringOption;
        }
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        public static bool InitializePrefix(StringOption __instance)
        {
            if (__instance.name == "ModdedOption")
            {
                __instance.TitleText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Title);
                __instance.ValueText.text = DestroyableSingleton<TranslationController>.Instance.GetString(__instance.Values[__instance.Value]);
                __instance.AdjustButtonsActiveState();
                return false;
            }
            return true;
        }
        [HarmonyPatch("UpdateValue")]
        [HarmonyPrefix]
        public static bool UpdateValuePrefix(StringOption __instance)
        {
            return __instance.name != "ModdedOption";
        }
    }
}
