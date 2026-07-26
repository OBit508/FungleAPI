using BepInEx.Configuration;
using Epic.OnlineServices.RTC;
using FungleAPI.Extensions;
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

namespace FungleAPI.GameOptions.Attributes
{
    public class ModdedToggleOptionAttribute : BaseModdedOptionAttribute
    {
        public bool LocalValue;
        public bool NonHostValue;

        public override object GetReturnedValue() => AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue;

        public override void SetValue(object value, bool amHost)
        {
            bool realValue = amHost ? LocalValue : NonHostValue;

            if (value is bool boolValue) { realValue = boolValue; }

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
            messageWriter.Write(LocalValue);
        }
        public override void Deserialize(MessageReader messageReader)
        {
            NonHostValue = messageReader.ReadBoolean();
        }
        public override void SaveValue(ConfigEntry<string> configEntry)
        {
            configEntry.Value = LocalValue.ToString();
        }
        public override void LoadValue(ConfigEntry<string> configEntry)
        {
            LocalValue = bool.Parse(configEntry.Value);
        }
        public override OptionBehaviour CreateOption(Transform transform)
        {
            ToggleOption toggleOption = UnityEngine.Object.Instantiate(PrefabUtils.FindPrefab<ToggleOption>(), Vector3.zero, Quaternion.identity, transform);
            toggleOption.SetUpFromData(Data, 20);
            toggleOption.Title = Data.Title;
            toggleOption.TitleText.text = Data.Title.GetString();
            toggleOption.oldValue = LocalValue;
            toggleOption.CheckMark.enabled = toggleOption.oldValue;
            toggleOption.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                SetValue(toggleOption.CheckMark.enabled, true);
            });
            OptionManager.FixOption(toggleOption);
            return toggleOption;
        }
        public ModdedToggleOptionAttribute(string defaultName)
        {
            Data = ScriptableObject.CreateInstance<CheckboxGameSetting>().DontUnload();
            CheckboxGameSetting checkboxGameSetting = (CheckboxGameSetting)Data;
            checkboxGameSetting.Title = TranslationManager.GetStringName(defaultName);
            checkboxGameSetting.Type = OptionTypes.Checkbox;
        }
    }
}
