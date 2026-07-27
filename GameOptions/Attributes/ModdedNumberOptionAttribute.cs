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
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.GameOptions.Attributes
{
    public class ModdedNumberOptionAttribute : BaseModdedOptionAttribute
    {
        public float LocalValue;
        public float NonHostValue;

        public override object GetReturnedValue()
        {
            float realValue = AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue;

            if (ReturnedType == typeof(float))
            {
                return realValue;
            }
            return (int)realValue;
        }

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
            else
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
            var setting = Data.SafeCast<FloatGameSetting>();
            float increment = setting.Increment > 0f ? setting.Increment : 1f;
            float clamped = Mathf.Clamp(value, setting.ValidRange.min, setting.ValidRange.max);

            float defaultV = (float)DefaultValue;
            float steps = Mathf.Round((clamped - defaultV) / increment);
            float snapped = defaultV + steps * increment;

            return Mathf.Clamp(snapped, setting.ValidRange.min, setting.ValidRange.max);
        }
        public ModdedNumberOptionAttribute(string defaultName, float minValue, float maxValue, float increment = 1, string formatString = null, bool zeroIsInfinity = false, NumberSuffixes suffixType = NumberSuffixes.Seconds)
        {
            Data = ScriptableObject.CreateInstance<FloatGameSetting>().DontUnload();
            FloatGameSetting floatGameSetting = (FloatGameSetting)Data;
            floatGameSetting.Title = TranslationManager.GetStringName(defaultName);
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
