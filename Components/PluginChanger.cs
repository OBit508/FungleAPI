using FungleAPI.Api;
using FungleAPI.Attributes;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    /// The component used in the lobby tab selectors
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class PluginChanger : MonoBehaviour
    {
        public int CurrentIndex;
        public Assembly CurrentPlugin;
        public IEnumerable<Assembly> Plugins;
        public Action<Assembly> OnChange;
        public TextMeshPro Text;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public void Awake()
        {
            Text = GetComponentInChildren<TextMeshPro>();
            RightButton = GetComponentsInChildren<PassiveButton>()[0];
            LeftButton = GetComponentsInChildren<PassiveButton>()[1];
            CurrentPlugin = FungleApiPlugin.Plugin.ModAssembly;

            void up()
            {
                if (Plugins.Count() <= 0) return;

                CurrentPlugin = Plugins.ElementAt(CurrentIndex);
                Text.text = GetCurrentName() + GetSubText();
                OnChange?.Invoke(CurrentPlugin);
            }

            RightButton.SetNewAction(new Action(delegate
            {
                if ((CurrentIndex + 1) >= Plugins.Count())
                {
                    CurrentIndex = 0;
                }
                else
                {
                    CurrentIndex++;
                }
                up();
            }));
            LeftButton.SetNewAction(new Action(delegate
            {
                if ((CurrentIndex - 1) <= -1)
                {
                    CurrentIndex = Plugins.Count() - 1;
                }
                else
                {
                    CurrentIndex--;
                }
                up();
            }));
        }
        public void Start()
        {
            Text.text = "Vanilla" + GetSubText();
        }
        public string GetCurrentName()
        {
            ModPlugin modPlugin = ModPluginManager.GetModPlugin(CurrentPlugin);
            if (modPlugin != null)
            {
                return modPlugin.FunglePlugin.ModName;
            }
            return BepInMod.GetMod(CurrentPlugin).Name;
        }
        public string GetSubText()
        {
            return $"\n<size=40%>{Color.yellow.ToTextColor()}({CurrentIndex + 1}/{Plugins.Count()})</color></size>";
        }
    }
}
