using BepInEx.Configuration;
using FungleAPI.Attributes;
using FungleAPI.PluginLoading;
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
    public abstract class BaseModdedOptionAttribute : Attribute, IModdedOption
    {
        public Action OnValueChance;
        public Type ReturnedType;
        public ConfigEntry<string> Entry { get; set; }
        public BaseGameSetting Data { get; set; }
        public object DefaultValue { get; set; }
        public uint OptionId { get; set; }
        public string StringOptionId { get; set; }
        public ModPlugin OwnerPlugin { get; set; }
        public abstract object GetReturnedValue();
        public void SetOnValueChance(Action action) => OnValueChance += action;
        public abstract void SetValue(object value, bool amHost);
        public abstract string GetStringValue(bool amHost);
        public abstract void Serialize(MessageWriter messageWriter);
        public abstract void Deserialize(MessageReader messageReader);
        public abstract void SaveValue(ConfigEntry<string> configEntry);
        public abstract void LoadValue(ConfigEntry<string> configEntry);
        public abstract OptionBehaviour CreateOption(Transform parent);
        public virtual void Initialize(PropertyInfo propertyInfo)
        {
            StringOptionId = $"{propertyInfo.Name}.{propertyInfo.DeclaringType.GetShortUniqueId()}";
            ReturnedType = propertyInfo.PropertyType;
            DefaultValue = propertyInfo.GetValue(null);
            TranslationHelper attributeTranslationID = propertyInfo.GetCustomAttribute<TranslationHelper>();
            if (attributeTranslationID != null && TranslationManager.TranslationIDs.TryGetValue(attributeTranslationID.TranslationID, out Translator translator))
            {
                Data.Title = translator.StringName;
            }
        }
    }
}
