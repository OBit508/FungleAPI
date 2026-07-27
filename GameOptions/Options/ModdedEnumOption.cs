using BepInEx.Configuration;
using Epic.OnlineServices.RTC;
using FungleAPI.Assets.Late;
using FungleAPI.Extensions;
using FungleAPI.GameOptions.Attributes;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using Il2CppSystem.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace FungleAPI.GameOptions.Options
{
    public class ModdedEnumOption<TEnum> : BaseModdedOption where TEnum : Enum
    {
        public int LocalValue;
        public int NonHostValue;

        public Dictionary<int, TEnum> Values = new Dictionary<int, TEnum>();

        public TEnum EnumValue => Values[AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue];

        public override void SetValue(object value, bool amHost)
        {
            int realValue = amHost ? LocalValue : NonHostValue;

            if (value is int intValue) { realValue = intValue; }

            if (amHost)
            {
                LocalValue = realValue;
                SaveValue(Entry);
            }
            else
            {
                NonHostValue = realValue;
            }
            OnValueChance?.Invoke();
        }
        public override string GetStringValue(bool amHost)
        {
            StringGameSetting stringGameSetting = Data.SafeCast<StringGameSetting>();
            if (amHost) return stringGameSetting.Values[LocalValue].GetString();
            return stringGameSetting.Values[NonHostValue].GetString();
        }
        public override void Serialize(MessageWriter messageWriter)
        {
            if (Values.Count <= byte.MaxValue)
            {
                messageWriter.Write((byte)LocalValue);
                return;
            }
            messageWriter.Write((ushort)LocalValue);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            if (Values.Count <= byte.MaxValue)
            {
                NonHostValue = messageReader.ReadByte();
            }
            else
            {
                NonHostValue = messageReader.ReadUInt16();
            }

            if ((Values.Count - 1) < NonHostValue)
            {
                NonHostValue = Values.Count - 1;
            }
        }
        public override void SaveValue(ConfigEntry<string> configEntry)
        {
            configEntry.Value = LocalValue.ToString();
        }
        public override void LoadValue(ConfigEntry<string> configEntry)
        {
            LocalValue = int.Parse(configEntry.Value);
            if ((Values.Count - 1) < LocalValue)
            {
                LocalValue = Values.Count - 1;
            }
        }
        public override OptionBehaviour CreateOption(Transform parent)
        {
            StringGameSetting stringGameSetting = Data.SafeCast<StringGameSetting>();
            StringOption stringOption = null;
            stringOption = OptionManager.CreateEnumOption(parent, stringGameSetting, delegate
            {
                SetValue(stringOption.Value, true);
                stringGameSetting.Index = stringOption.Value;
            });
            stringOption.Value = LocalValue;
            return stringOption;
        }
        public ModdedEnumOption(StringNames optionName, TEnum defaultValue, Dictionary<TEnum, StringNames> valuesNames)
        {
            DefaultValue = valuesNames.Keys.GetIndex(defaultValue);
            Data = ScriptableObject.CreateInstance<StringGameSetting>().DontUnload();
            StringGameSetting stringGameSetting = (StringGameSetting)Data;
            stringGameSetting.Type = OptionTypes.String;
            stringGameSetting.Title = optionName;
            stringGameSetting.Values = valuesNames.Values.ToArray();

            int i = 0;
            foreach (TEnum @enum in valuesNames.Keys)
            {
                Values.Add(i, @enum);
                i++;
            }
        }
        public ModdedEnumOption(string optionName, TEnum defaultValue, Dictionary<TEnum, string> valuesNames)
            :this(TranslationManager.GetStringName(optionName), defaultValue, new Dictionary<TEnum, StringNames>())
        {
            Values.Clear();

            StringGameSetting stringGameSetting = (StringGameSetting)Data;

            stringGameSetting.Values = new StringNames[valuesNames.Count];

            int i = 0;
            foreach (KeyValuePair<TEnum, string> pair in valuesNames)
            {
                Values.Add(i, pair.Key);
                stringGameSetting.Values[i] = TranslationManager.GetStringName(pair.Value);
            }
        }
        public static implicit operator TEnum(ModdedEnumOption<TEnum> moddedEnumOption)
        {
            return moddedEnumOption.EnumValue;
        }
    }
}
