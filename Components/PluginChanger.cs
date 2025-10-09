using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Components
{
    [Attributes.RegisterTypeInIl2Cpp]
    public class PluginChanger : MonoBehaviour
    {
        public int CurrentIndex;
        public ModPlugin CurrentPlugin;
        public Action<ModPlugin> OnChange;
        public TextMeshPro Text;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public bool initialized;
        public void Initialize()
        {
            if (!initialized)
            {
                Text = GetComponentInChildren<TextMeshPro>();
                RightButton = GetComponentsInChildren<PassiveButton>()[0];
                LeftButton = GetComponentsInChildren<PassiveButton>()[1];
                CurrentPlugin = FungleAPIPlugin.Plugin;
                Text.text = CurrentPlugin.ModName;
                RightButton.ClickSound = EOSManager.Instance.askToMergeAccount.NotRightNowButton.ClickSound;
                RightButton.GetComponent<ButtonRolloverHandler>().HoverSound = EOSManager.Instance.askToMergeAccount.NotRightNowButton.GetComponent<ButtonRolloverHandler>().HoverSound;
                RightButton.SetNewAction(new Action(delegate
                {
                    if ((CurrentIndex + 1) >= ModPlugin.AllPlugins.Count)
                    {
                        CurrentIndex = 0;
                    }
                    else
                    {
                        CurrentIndex++;
                    }
                    CurrentPlugin = ModPlugin.AllPlugins[CurrentIndex];
                    Text.text = CurrentPlugin.ModName;
                    OnChange?.Invoke(CurrentPlugin);
                }));
                LeftButton.ClickSound = RightButton.ClickSound;
                LeftButton.GetComponent<ButtonRolloverHandler>().HoverSound = RightButton.GetComponent<ButtonRolloverHandler>().HoverSound;
                LeftButton.SetNewAction(new Action(delegate
                {
                    if ((CurrentIndex - 1) <= -1)
                    {
                        CurrentIndex = ModPlugin.AllPlugins.Count - 1;
                    }
                    else
                    {
                        CurrentIndex--;
                    }
                    CurrentPlugin = ModPlugin.AllPlugins[CurrentIndex];
                    Text.text = CurrentPlugin.ModName;
                    OnChange?.Invoke(CurrentPlugin);
                }));
            }
        }
        public void Start()
        {
            Initialize();
        }
    }
}
