using BepInEx.Configuration;
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
        public BaseGameSetting Data;
        public Translator ConfigName;
        public PropertyInfo Property;
        internal string FullConfigName;
        internal string onlineValue;
        internal ConfigEntry<string> localValue;
        protected ModdedOption(string configName)
        {
            ConfigName = new Translator(configName);
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
        }
        public virtual object GetReturnedValue()
        {
            return null;
        }
    }
}
