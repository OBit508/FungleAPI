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
        public TextMeshPro PageText;
        public int Page;
        public List<List<(string str, string link)>> Pages;
        public List<TextMeshPro> Texts;
        public List<PassiveButton> Buttons;
        public List<ButtonRolloverHandler> Rollovers;
        public List<BoxCollider2D> Colliders;
        public Color linkColor = new Color32(52, 152, 235, byte.MaxValue);
        public void Awake()
        {
            transform.localPosition = Closed;
            Pages = new List<List<(string str, string link)>>() { new List<(string str, string link)>() };
            Texts = new List<TextMeshPro>();
            Buttons = new List<PassiveButton>();
            Rollovers = new List<ButtonRolloverHandler>();
            Colliders = new List<BoxCollider2D>();
            TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();
            PassiveButton[] buttons = GetComponentsInChildren<PassiveButton>();
            Arrow = buttons[0];
            RightButton = buttons[1];
            LeftButton = buttons[2];
            CreditsText = texts[0];
            PageText = texts[1];
            foreach (ModPlugin plugin in ModPlugin.AllPlugins)
            {
                List<(string str, string link)> strings = Pages[Pages.Count - 1];
                strings.Add((plugin.ModCredits, plugin.link));
                if (strings.Count >= 10)
                {
                    Pages.Add(new List<(string str, string link)>());
                }
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
            for (int i = 2; i < texts.Count(); i++)
            {
                Texts.Add(texts[i]);
                Buttons.Add(texts[i].GetComponent<PassiveButton>());
                Rollovers.Add(texts[i].GetComponent<ButtonRolloverHandler>());
                Colliders.Add(texts[i].GetComponent<BoxCollider2D>());
            }
            UpdatePage();
        }
        public void Update()
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Opening ? Open : Closed, Time.deltaTime * 12);
        }
        public void UpdatePage()
        {
            foreach (TextMeshPro text in Texts)
            {
                text.text = null;
            }
            List<(string str, string link)> currentPage = Pages[Page];
            int count = Mathf.Min(Texts.Count, currentPage.Count);
            for (int i = 0; i < count; i++)
            {
                (string str, string link) pair = currentPage[i];
                Texts[i].text = pair.str;
                Rollovers[i].OverColor = !string.IsNullOrEmpty(pair.link) ? linkColor : Color.white;
                Colliders[i].size = new Vector2(Mathf.Min(2.5f * pair.str.Length, 50), 3);
                string currentLink = pair.link;
                Buttons[i].enabled = !string.IsNullOrEmpty(currentLink);
                Buttons[i].SetNewAction(() =>
                {
                    if (!string.IsNullOrEmpty(currentLink))
                    {
                        Application.OpenURL(currentLink);
                    }
                });
            }
            PageText.text = (Page + 1).ToString() + "/" + Pages.Count.ToString();
        }
    }
}
