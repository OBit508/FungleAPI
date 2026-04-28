using FungleAPI.Attributes;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ModdedOptionAttribute : Attribute, IModdedOption
    {
        public Action<bool> OnValueChance;
        public Type ReturnedType;
        public BaseGameSetting Data { get; set; }
        public object DefaultValue { get; set; }
        public string OptionId { get; set; }
        public abstract object GetReturnedValue();
        public void SetOnValueChance(Action<bool> action) => OnValueChance += action;
        public abstract void SetValue(object value, bool amHost);
        public abstract string GetStringValue(bool amHost);
        public abstract void Serialize(MessageWriter messageWriter);
        public abstract void Deserialize(MessageReader messageReader);
        public abstract void WriteLocalValue(BinaryWriter binaryWriter);
        public abstract void ReadLocalValue(BinaryReader binaryReader);
        public abstract void SyncNonHostWithLocal();
        public abstract OptionBehaviour CreateOption(Transform parent);
        public virtual void Initialize(PropertyInfo propertyInfo)
        {
            ReturnedType = propertyInfo.PropertyType;
            DefaultValue = propertyInfo.GetValue(null);
            OptionId = $"{propertyInfo.DeclaringType.Name}:{propertyInfo.Name}:{propertyInfo.DeclaringType.GetShortUniqueId()}";
            TranslationHelper attributeTranslationID = propertyInfo.GetCustomAttribute<TranslationHelper>();
            if (attributeTranslationID != null && TranslationManager.TranslationIDs.TryGetValue(attributeTranslationID.TranslationID, out Translator translator))
            {
                Data.Title = translator.StringName;
            }
        }
    }
}
