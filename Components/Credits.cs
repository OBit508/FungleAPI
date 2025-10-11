using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using xCloud;

namespace FungleAPI.Components
{
    [Attributes.RegisterTypeInIl2Cpp]
    public class Credits : MonoBehaviour
    {
        public Vector3 Closed =  new Vector3(5.55f, 0, 0);
        public Vector3 Open = new Vector3(1.5f, 0, 0);
        public bool Opening;
        public PassiveButton Arrow;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public TextMeshPro CreditsText;
        public TextMeshPro ModsText;
        public TextMeshPro PageText;
        public int Page;
        public List<List<string>> Pages;
        public void Awake()
        {
            AudioClip hoverSound = EOSManager.Instance.askToMergeAccount.NotRightNowButton.GetComponent<ButtonRolloverHandler>().HoverSound;
            AudioClip clickSound = EOSManager.Instance.askToMergeAccount.NotRightNowButton.ClickSound;
            transform.localPosition = Closed;
            Pages = new List<List<string>>() { new List<string>() };
            TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();
            PassiveButton[] buttons = GetComponentsInChildren<PassiveButton>();
            Arrow = buttons[0];
            RightButton = buttons[1];
            LeftButton = buttons[2];
            CreditsText = texts[0];
            ModsText = texts[1];
            PageText = texts[2];
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                List<string> strings = Pages[Pages.Count - 1];
                strings.Add((plugin == ModPlugin.AllPlugins[0] ? "" : "\n") + plugin.ModCredits);
                if (strings.Count >= 10)
                {
                    Pages.Add(new List<string>());
                }
            }
            foreach (PassiveButton button in buttons)
            {
                button.HoverSound = hoverSound;
                button.ClickSound = clickSound;
            }
            Arrow.SetNewAction(new Action(delegate
            {
                Opening = !Opening;
                CreditsText.text = "<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">" + FungleTranslation.CreditsText.GetString() + "</font>";
            }));
            RightButton.SetNewAction(new Action(delegate
            {
                if (Page + 1 >= Pages.Count)
                {
                    return;
                }
                Page++;
                UpdatePage();
            }));
            LeftButton.SetNewAction(new Action(delegate
            {
                if (Page - 1 <= -1)
                {
                    return;
                }
                Page--;
                UpdatePage();
            }));
            UpdatePage();
        }
        public void Update()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Opening ? Open : Closed, Time.deltaTime * 12);
        }
        public void UpdatePage()
        {
            ModsText.text = "";
            foreach (string str in Pages[Page])
            {
                ModsText.text += str;
            }
            PageText.text = (Page + 1).ToString() + "/" + Pages.Count.ToString();
        }
    }
}
