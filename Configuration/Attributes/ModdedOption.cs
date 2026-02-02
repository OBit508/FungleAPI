using BepInEx.Configuration;
using FungleAPI.Attributes;
using FungleAPI.Translation;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace FungleAPI.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedOption : Attribute
    {
        private Translator __translator;
        private string __translationID;
        public BaseGameSetting Data;
        public PropertyInfo Property;
        internal string FullConfigName;
        internal string onlineValue;
        internal ConfigEntry<string> localValue;
        protected ModdedOption(string configName)
        {
            __translator = new Translator(configName);
        }
        public Translator ConfigName
        {
            get
            {
                if (__translationID != null && TranslationManager.TranslationIDs.TryGetValue(__translationID, out Translator translator))
                {
                    return translator;
                }
                return __translator;
            }
        }
        public string GetValue()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localValue.Value;
            }
            else
            {
                return onlineValue;
            }
        }
        public void SetValue(object value)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                localValue.Value = value.ToString();
            }
            else
            {
                onlineValue = value.ToString();
            }
        }
        public static void SetUpFromData(OptionBehaviour option, int maskLayer = 20)
        {
            SpriteRenderer[] componentsInChildren = option.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            }
            foreach (TextMeshPro textMeshPro in option.GetComponentsInChildren<TextMeshPro>(true))
            {
                textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                textMeshPro.fontMaterial.SetFloat("_Stencil", maskLayer);
                textMeshPro.enabled = true;
            }
            option.enabled = false;
        }
        public static void FixOption(OptionBehaviour option)
        {
            foreach (TextMeshPro textMeshPro in option.GetComponentsInChildren<TextMeshPro>(true))
            {
                textMeshPro.enabled = true;
            }
            foreach (GameOptionButton gameOptionButton in option.GetComponentsInChildren<GameOptionButton>(true))
            {
                gameOptionButton.enabled = true;
            }
            foreach (PassiveButton passiveButton in option.GetComponentsInChildren<PassiveButton>(true))
            {
                passiveButton.enabled = true;
            }
            foreach (BoxCollider2D boxCollider2D in option.GetComponentsInChildren<BoxCollider2D>(true))
            {
                boxCollider2D.enabled = true;
            }
            option.GetComponent<UIScrollbarHelper>().enabled = true;
            option.LabelBackground.enabled = false;
            option.name = "ModdedOption";
            option.enabled = true;
        }
        public virtual OptionBehaviour CreateOption(Transform transform)
        {
            return null;
        }
        public virtual void Initialize(PropertyInfo property)
        {
            Property = property;
            TranslationHelper attributeTranslationID = property.GetCustomAttribute<TranslationHelper>();
            if (attributeTranslationID != null)
            {
                __translationID = attributeTranslationID.TranslationID;
            }
        }
        public virtual object GetReturnedValue()
        {
            return null;
        }
    }
}
