using FungleAPI.Attributes;
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
using static Il2CppSystem.DateTimeParse;

namespace FungleAPI.Components
{
    /// <summary>
    /// The component used on the home screen mods page
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class ModsPage : MonoBehaviour
    {
        public PassiveButton Arrow;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public TextMeshPro PageText;
        public int Page;
        public List<List<(string, Action)>> Pages;
        public TextMeshPro[] Texts;
        public float p;
        public void Awake()
        {
            Pages = new List<List<(string, Action)>>() { new List<(string, Action)>() };
            Texts = GetComponentsInChildren<TextMeshPro>();
            PassiveButton[] buttons = GetComponentsInChildren<PassiveButton>();
            RightButton = buttons[0];
            LeftButton = buttons[1];
            Texts[0].text = "<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">Mods:</font>";
            PageText = Texts[1];
            foreach (ModPlugin plugin in ModPluginManager.AllPlugins)
            {
                List<(string, Action)> strings = Pages[Pages.Count - 1];

                Action click = null;

                IFungleBasePlugin fungleBasePlugin = plugin.BasePlugin as IFungleBasePlugin;
                if (fungleBasePlugin != null)
                {
                    click = fungleBasePlugin.ClickOnModName;
                }
                else if (plugin == FungleAPIPlugin.Plugin)
                {
                    click = FungleAPIPlugin.OpenCreditsScreen;
                }

                strings.Add((plugin.ModCredits, click));
                if (strings.Count >= 10)
                {
                    Pages.Add(new List<(string, Action)>());
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
        /// <summary>
        /// Update the mods page
        /// </summary>
        public void UpdatePage()
        {
            List<(string, Action)> currentPage = Pages[Page];
            for (int i = 2; i < Texts.Length; i++)
            {
                TextMeshPro textMeshPro = Texts[i];
                textMeshPro.text = null;
                textMeshPro.GetComponent<BoxCollider2D>().enabled = false;
                textMeshPro.GetComponent<ButtonRolloverHandler>().enabled = false;
            }
            for (int i = 0; i < currentPage.Count && i + 2 < Texts.Length; i++)
            {
                int index = i;
                TextMeshPro textMeshPro = Texts[i + 2];
                textMeshPro.text = currentPage[index].Item1;
                BoxCollider2D boxCollider2D = textMeshPro.GetComponent<BoxCollider2D>();
                boxCollider2D.enabled = currentPage[index].Item2 != null;
                boxCollider2D.GetComponent<ButtonRolloverHandler>().enabled = boxCollider2D.enabled;
                if (boxCollider2D.enabled)
                {
                    textMeshPro.ForceMeshUpdate();
                    Bounds b = textMeshPro.textBounds;
                    boxCollider2D.size = new Vector2(b.size.x, b.size.y);
                    boxCollider2D.offset = new Vector2(b.center.x, b.center.y);
                    boxCollider2D.GetComponent<PassiveButton>().SetNewAction(currentPage[index].Item2);
                }
            }
            PageText.text = (Page + 1).ToString() + "/" + Pages.Count;
        }
    }
}
