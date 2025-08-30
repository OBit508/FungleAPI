using BepInEx.Configuration;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using FungleAPI.Translation;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModdedOption : Attribute
    {
        public StringNames ConfigName;
        internal string FullConfigName;
        internal string onlineValue;
        internal ConfigEntry<string> localValue;
        protected ModdedOption(string configName)
        {
            ConfigName = new Translator(configName).StringName;
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
        public void SetUpFromData(OptionBehaviour option, int maskLayer = 20)
        {
            SpriteRenderer[] componentsInChildren = option.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, maskLayer);
            }
            foreach (TextMeshPro textMeshPro in option.GetComponentsInChildren<TextMeshPro>(true))
            {
                textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                textMeshPro.fontMaterial.SetFloat("_Stencil", (float)maskLayer);
                textMeshPro.enabled = true;
            }
            option.enabled = false;
        }
        public virtual OptionBehaviour CreateOption(Transform transform)
        {
            return null;
        }
        public virtual void Initialize(Type type, PropertyInfo property, object obj)
        {
        }
    }
}
