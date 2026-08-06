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

        public override void SetValue(object value, bool amHost)
        {
            float realValue = amHost ? LocalValue : NonHostValue;

            if (value is float floatValue) { realValue = floatValue; }
            if (value is int intValue) { realValue = intValue; }

            realValue = Quantize(realValue);

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
            var setting = Data.SafeCast<FloatGameSetting>();
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
            var setting = Data.SafeCast<FloatGameSetting>();
            float increment = setting.Increment > 0f ? setting.Increment : 1f;
            float defaultV = (float)DefaultValue;

            byte marker = messageReader.ReadByte();

            switch (marker)
            {
                case 2:
                    NonHostValue = defaultV;
                    return;

                case 3:
                    NonHostValue = messageReader.ReadSingle();
                    return;

                case 0:
                case 1:
                    ushort magnitude = messageReader.ReadUInt16();
                    int delta = marker == 1 ? magnitude : -magnitude;
                    NonHostValue = defaultV + delta * increment;
                    return;
            }
        }
        public override void SaveValue(ConfigEntry<string> configEntry)
        {
            configEntry.Value = LocalValue.ToString();
        }
        public override void LoadValue(ConfigEntry<string> configEntry)
        {
            LocalValue = Quantize(float.Parse(configEntry.Value));
        }
        public override OptionBehaviour CreateOption(Transform parent)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            NumberOption option = null;
            option = OptionManager.CreateNumberOption(parent, setting, delegate
            {
                SetValue(option.Value, true);
                setting.Value = option.Value;
            });
            setting.Value = LocalValue;
            option.Value = LocalValue;
            return option;
        }
        private float Quantize(float value)
        {
            FloatGameSetting setting = Data.SafeCast<FloatGameSetting>();
            float increment = setting.Increment > 0f ? setting.Increment : 1f;
            float clamped = Mathf.Clamp(value, setting.ValidRange.min, setting.ValidRange.max);
            float defaultV = (float)DefaultValue;
            float steps = Mathf.Round((clamped - defaultV) / increment);
            float snapped = defaultV + steps * increment;
            snapped = Mathf.Clamp(snapped, setting.ValidRange.min, setting.ValidRange.max);
            int decimals = GetDecimalPlaces(increment);
            snapped = (float)Math.Round(snapped, decimals, MidpointRounding.AwayFromZero);
            return snapped;
        }
        private static int GetDecimalPlaces(float increment)
        {
            string s = increment.ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
            int idx = s.IndexOf('.');
            if (idx < 0) return 0;
            return Math.Min(s.Length - idx - 1, 6);
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
