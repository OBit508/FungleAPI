using Epic.OnlineServices.RTC;
using FungleAPI.Extensions;
using FungleAPI.GameOptions.Attributes;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
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
    public class ModdedEnumOption<TEnum> : ModdedOption where TEnum : Enum
    {
        public int LocalValue;
        public int NonHostValue;

        public Dictionary<int, TEnum> Values = new Dictionary<int, TEnum>();

        public TEnum EnumValue => Values[AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue];

        public override void SetValue(object value, bool amHost)
        {
            bool changed = false;
            int realValue = amHost ? LocalValue : NonHostValue;

            if (value is int intValue) { changed = realValue != intValue; realValue = intValue; }

            if (amHost)
            {
                LocalValue = realValue;
                OnValueChance?.Invoke(changed);
                return;
            }
            NonHostValue = realValue;
        }
        public override string GetStringValue(bool amHost)
        {
            StringGameSetting stringGameSetting = Data.SafeCast<StringGameSetting>();
            if (amHost) return stringGameSetting.Values[LocalValue].GetString();
            return stringGameSetting.Values[NonHostValue].GetString();
        }
        public override void Serialize(MessageWriter messageWriter)
        {
            messageWriter.WritePacked(LocalValue);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            NonHostValue = messageReader.ReadPackedInt32();
            if ((Values.Count - 1) < NonHostValue)
            {
                NonHostValue = Values.Count - 1;
            }
        }
        public override void WriteLocalValue(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(LocalValue);
        }
        public override void ReadLocalValue(BinaryReader binaryReader)
        {
            LocalValue = binaryReader.ReadInt32();
            if ((Values.Count - 1) < LocalValue)
            {
                LocalValue = Values.Count - 1;
            }
        }
        public override void SyncNonHostWithLocal()
        {
            NonHostValue = LocalValue;
        }
        public override OptionBehaviour CreateOption(Transform parent)
        {
            StringGameSetting stringGameSetting = Data.SafeCast<StringGameSetting>();
            StringOption stringOption = OptionManager.CreateEnumOption(parent, stringGameSetting, delegate (StringOption stringOption)
            {
                SetValue(stringOption.Value, true);
                stringGameSetting.Index = stringOption.Value;
            });
            stringOption.Value = LocalValue;
            return stringOption;
        }
        public ModdedEnumOption(StringNames optionName, TEnum defaultValue, Dictionary<TEnum, StringNames> valuesNames)
        {
            DefaultValue = defaultValue;
            Data = ScriptableObject.CreateInstance<StringGameSetting>().DontUnload();
            StringGameSetting stringGameSetting = (StringGameSetting)Data;
            stringGameSetting.Type = OptionTypes.String;
            stringGameSetting.Title = optionName;
            stringGameSetting.Values = valuesNames.Values.ToArray();
            for (int i = 0; i < valuesNames.Keys.Count; i++)
            {
                Values.Add(i, valuesNames.Keys.ElementAt(i));
            }
        }
    }
}
