using AmongUs.GameOptions;
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

namespace FungleAPI.GameOptions.Options
{
    public class ModdedNumberOption : ModdedOption
    {
        public float LocalValue;
        public float NonHostValue;

        public int IntValue => (int)FloatValue;
        public float FloatValue => AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue;

        public override void SetValue(object value, bool amHost)
        {
            bool changed = false;
            float realValue = amHost ? LocalValue : NonHostValue;

            if (value is float floatValue) { changed = realValue != floatValue; realValue = floatValue; }
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
            if (amHost) return LocalValue.ToString();
            return NonHostValue.ToString();
        }
        public override void Serialize(MessageWriter messageWriter)
        {
            messageWriter.Write(LocalValue);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            NonHostValue = messageReader.ReadSingle();
        }
        public override void WriteLocalValue(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(LocalValue);
        }
        public override void ReadLocalValue(BinaryReader binaryReader)
        {
            LocalValue = binaryReader.ReadSingle();
        }
        public override void SyncNonHostWithLocal()
        {
            NonHostValue = LocalValue;
        }
        public override OptionBehaviour CreateOption(Transform parent)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            NumberOption option = OptionManager.CreateNumberOption(parent, setting, delegate (NumberOption option)
            {
                SetValue(option.Value, true);
                setting.Value = option.Value;
            });
            setting.Value = LocalValue;
            option.Value = LocalValue;
            return option;
        }
        public ModdedNumberOption(StringNames optionName, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
        {
            Data = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            FloatGameSetting floatGameSetting = (FloatGameSetting)Data;
            floatGameSetting.Title = optionName;
            floatGameSetting.Type = OptionTypes.Float;
            floatGameSetting.Increment = increment;
            floatGameSetting.ValidRange = new FloatRange(minValue, maxValue);
            floatGameSetting.FormatString = formatString;
            floatGameSetting.ZeroIsInfinity = zeroIsInfinity;
            floatGameSetting.SuffixType = suffixType;
            floatGameSetting.OptionName = FloatOptionNames.Invalid;
        }
    }
}
