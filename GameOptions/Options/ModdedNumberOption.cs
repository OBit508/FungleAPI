using AmongUs.GameOptions;
using BepInEx.Configuration;
using FungleAPI.Extensions;
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
    public class ModdedNumberOption : BaseModdedOption
    {
        public float LocalValue;
        public float NonHostValue;

        public int IntValue => (int)FloatValue;
        public float FloatValue => AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue;
        public float Value => FloatValue;

        public override void SetValue(object value, bool amHost)
        {
            float realValue = amHost ? LocalValue : NonHostValue;

            if (value is float floatValue) { realValue = floatValue; }
            if (value is int intValue) { realValue = intValue; }

            realValue = OptionManager.Quantize(realValue, (float)DefaultValue, Data.SafeCast<FloatGameSetting>());

            if (amHost)
            {
                LocalValue = realValue;
                SaveValue(Entry);
            }
            {
                NonHostValue = realValue;
            }
            OnValueChance?.Invoke();
        }
        public override string GetStringValue(bool amHost)
        {
            if (amHost) return LocalValue.ToString();
            return NonHostValue.ToString();
        }
        public override void Serialize(MessageWriter messageWriter)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            float increment = setting.Increment > 0f ? setting.Increment : 1f;
            float defaultV = (float)DefaultValue;
            float clampedLocal = Mathf.Clamp(LocalValue, setting.ValidRange.min, setting.ValidRange.max);

            int delta = Mathf.RoundToInt((clampedLocal - defaultV) / increment);

            if (delta == 0)
            {
                messageWriter.Write((byte)2);
                return;
            }

            int magnitude = Mathf.Abs(delta);

            if (magnitude > ushort.MaxValue)
            {
                messageWriter.Write((byte)3);
                messageWriter.Write(clampedLocal);
                return;
            }

            byte sign = delta > 0 ? (byte)1 : (byte)0;
            messageWriter.Write(sign);
            messageWriter.Write((ushort)magnitude);
        }

        public override void Deserialize(MessageReader messageReader)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            float increment = setting.Increment > 0f ? setting.Increment : 1f;
            float defaultV = (float)DefaultValue;

            byte marker = messageReader.ReadByte();

            switch (marker)
            {
                case 2:
                    NonHostValue = OptionManager.Quantize(defaultV, defaultV, Data.SafeCast<FloatGameSetting>());
                    return;

                case 3:
                    NonHostValue = OptionManager.Quantize(messageReader.ReadSingle(), defaultV, Data.SafeCast<FloatGameSetting>());
                    return;

                case 0:
                case 1:
                    ushort magnitude = messageReader.ReadUInt16();
                    int delta = marker == 1 ? magnitude : -magnitude;
                    NonHostValue = OptionManager.Quantize(defaultV + delta * increment, defaultV, Data.SafeCast<FloatGameSetting>());
                    return;
            }
        }
        public override void SaveValue(ConfigEntry<string> configEntry)
        {
            configEntry.Value = LocalValue.ToString();
        }
        public override void LoadValue(ConfigEntry<string> configEntry)
        {
            LocalValue = OptionManager.Quantize(float.Parse(configEntry.Value), (float)DefaultValue, Data.SafeCast<FloatGameSetting>());
        }
        public override OptionBehaviour CreateOption(Transform parent)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            NumberOption option = null;
            option = OptionManager.CreateNumberOption(parent, setting, delegate
            {
                option.Value = OptionManager.Quantize(option.Value, (float)DefaultValue, setting);
                SetValue(option.Value, true);
                setting.Value = option.Value;
            });
            setting.Value = LocalValue;
            option.Value = LocalValue;
            return option;
        }
        public ModdedNumberOption(StringNames optionName, float defaultValue, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
        {
            DefaultValue = defaultValue;
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
        public ModdedNumberOption(string optionName, float defaultValue, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
            :this(TranslationManager.GetStringName(optionName), defaultValue, minValue, maxValue, increment, formatString, zeroIsInfinity, suffixType) { }
        public static implicit operator float(ModdedNumberOption moddedNumberOption)
        {
            return moddedNumberOption.FloatValue;
        }
        public static implicit operator int(ModdedNumberOption moddedNumberOption)
        {
            return moddedNumberOption.IntValue;
        }
    }
}
