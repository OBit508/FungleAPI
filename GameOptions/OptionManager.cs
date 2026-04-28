using AmongUs.GameOptions;
using FungleAPI.GameOptions.Attributes;
using FungleAPI.GameOptions.Collections;
using FungleAPI.GameOptions.Options;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.GameOptions
{
    public static class OptionManager
    {
        public static Dictionary<string, IModdedOption> AllOptions = new Dictionary<string, IModdedOption>();
        public static List<OptionCollection> OptionCollections = new List<OptionCollection>();

        public static void SaveOptionCollections()
        {
            foreach (OptionCollection optionCollection in OptionCollections)
            {
                if (optionCollection.FilePath == null || !optionCollection.Dirty) continue;

                optionCollection.WriteLocalOptions();
                optionCollection.Dirty = false;
            }
        }

        public static List<IModdedOption> GetAndInitializeModdedOptions(Type type)
        {
            List<IModdedOption> moddedOptions = new List<IModdedOption>();

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                try
                {
                    if (propertyInfo.GetMethod != null && typeof(ModdedOption).IsSubclassOf(propertyInfo.PropertyType) && propertyInfo.GetValue(null) is ModdedOption option && option != null)
                    {
                        option.Initialize(propertyInfo);
                        moddedOptions.Add(option);
                        continue;
                    }

                    ModdedOptionAttribute moddedOptionAttribute = propertyInfo.GetCustomAttribute<ModdedOptionAttribute>();
                    MethodInfo method = propertyInfo.GetGetMethod(true);
                    if (moddedOptionAttribute != null)
                    {
                        moddedOptionAttribute.Initialize(propertyInfo);
                        moddedOptions.Add(moddedOptionAttribute);
                        HarmonyHelper.Patches.Add(method, new Func<object>(moddedOptionAttribute.GetReturnedValue));
                        FungleAPIPlugin.Harmony.Patch(method, new HarmonyMethod(typeof(HarmonyHelper).GetMethod("GetPrefix", BindingFlags.Static | BindingFlags.Public)));
                    }
                }
                catch { }
            }
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                try
                {
                    if (typeof(ModdedOption).IsSubclassOf(fieldInfo.FieldType) && fieldInfo.GetValue(null) is ModdedOption option && option != null)
                    {
                        option.Initialize(fieldInfo);
                        moddedOptions.Add(option);
                    }
                }
                catch { }
            }

            return moddedOptions;
        }

        public static NumberOption CreateNumberOption(Transform transform, FloatGameSetting data, Action<NumberOption> onChange)
        {
            NumberOption option = GameObject.Instantiate(PrefabUtils.FindPrefab<NumberOption>(), Vector3.zero, Quaternion.identity, transform);
            option.SetUpFromData(data, 20);
            option.OnValueChanged = new Action<OptionBehaviour>(delegate
            {
                onChange(option);
            });
            option.Title = data.Title;
            option.Increment = data.Increment;
            option.ValidRange = data.ValidRange;
            option.FormatString = data.FormatString;
            option.ZeroIsInfinity = data.ZeroIsInfinity;
            option.SuffixType = data.SuffixType;
            option.floatOptionName = FloatOptionNames.Invalid;
            FixOption(option);
            return option;
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
    }
}
