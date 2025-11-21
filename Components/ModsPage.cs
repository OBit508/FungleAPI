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
    public class ModsPage : MonoBehaviour
    {
        public Vector3 Closed;
        public Vector3 Open = new Vector3(-2.5f, -0.2f, 0);
        public bool Opening;
        public PassiveButton Arrow;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public TextMeshPro PageText;
        public int Page;
        public List<List<string>> Pages;
        public TextMeshPro[] Texts;
        public Color linkColor = new Color32(52, 152, 235, byte.MaxValue);
        public void Awake()
        {
            transform.localPosition = Closed;
            Pages = new List<List<string>>() { new List<string>() };
            Texts = GetComponentsInChildren<TextMeshPro>();
            PassiveButton[] buttons = GetComponentsInChildren<PassiveButton>();
            RightButton = buttons[0];
            LeftButton = buttons[1];
            Texts[0].text = "<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">Mods:</font>";
            PageText = Texts[1];
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                List<string> strings = Pages[Pages.Count - 1];
                strings.Add(plugin.ModCredits);
                if (strings.Count >= 10)
                {
                    Pages.Add(new List<string>());
                }
            }
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
            Vector3 pos = Opening ? Open : Closed;
            Vector3 scale = Opening ? Vector3.one * 0.9f : Vector3.zero;
            if (transform.localPosition != pos)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos, Time.deltaTime * 12);
            }
            if (transform.localScale != scale)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, scale, Time.deltaTime * 7);
            }
        }
        public void UpdatePage()
        {
            List<string> currentPage = Pages[Page];
            for (int i = 2; i < Texts.Length; i++)
            {
                Texts[i].text = null;
            }
            for (int i = 0; i < currentPage.Count && i + 2 < Texts.Length; i++)
            {
                Texts[i + 2].text = currentPage[i];
            }
            PageText.text = (Page + 1).ToString() + "/" + Pages.Count;
        }
    }
}
