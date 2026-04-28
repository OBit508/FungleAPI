using Epic.OnlineServices.RTC;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
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
    public class ModdedToggleOptionAttribute : ModdedOptionAttribute
    {
        public bool LocalValue;
        public bool NonHostValue;

        public override object GetReturnedValue() => AmongUsClient.Instance.AmHost ? LocalValue : NonHostValue;

        public override void SetValue(object value, bool amHost)
        {
            bool changed = false;
            bool realValue = amHost ? LocalValue : NonHostValue;

            if (value is bool boolValue) { changed = realValue != boolValue; realValue = boolValue; }

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
            NonHostValue = messageReader.ReadBoolean();
        }
        public override void WriteLocalValue(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(LocalValue);
        }
        public override void ReadLocalValue(BinaryReader binaryReader)
        {
            LocalValue = binaryReader.ReadBoolean();
        }
        public override void SyncNonHostWithLocal()
        {
            NonHostValue = LocalValue;
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
