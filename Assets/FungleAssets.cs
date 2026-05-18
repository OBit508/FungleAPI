using FungleAPI.Assets.Late;
using FungleAPI.Components;
using FungleAPI.Event;
using FungleAPI.Extensions;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
using xCloud;

namespace FungleAPI.Assets
{
    /// <summary>
    /// FungleAPI assets
    /// </summary>
    public static class FungleAssets
    {
        private static Transform Parent = new GameObject("Fungle Prefabs") { active = false }.DontDestroy().transform;
        public static LateSprite Cog = new LateSprite("FungleAPI.Assets.FungleAssets.cog.png", 90);
        public static LateSprite Empty = new LateSprite("FungleAPI.Assets.FungleAssets.empty.png", 100);
        public static LateSprite NextButton = new LateSprite("FungleAPI.Assets.FungleAssets.nextButton.png", 100);
        public static LateSprite PluginChangerBackground =  new LateSprite("FungleAPI.Assets.FungleAssets.pluginChangerBackground.png", 100);
        public static LateAudio HoverSound = new LateAudio("FungleAPI.Assets.FungleAssets.UI_Hover.wav");
        public static LateAudio SelectSound = new LateAudio("FungleAPI.Assets.FungleAssets.UI_Select.wav");
        public static PluginChanger PluginChangerPrefab;
        public static ModsPage ModsPagePrefab;
        public static PassiveButton CogPrefab;
        public static void LoadAll()
        {
            CreatePluginChanger();
            CreateModsPage();
            CreateCog();
        }
        private static void CreateCog()
        {
            CogPrefab = new GameObject("Cog") { transform = { parent = Parent } }.AddComponent<PassiveButton>();
            CogPrefab.ClickSound = SelectSound;
            BoxCollider2D boxCollider2D = CogPrefab.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            SpriteRenderer rend = CogPrefab.gameObject.AddComponent<SpriteRenderer>();
            rend.sprite = Cog;
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            ButtonRolloverHandler buttonRolloverHandler = CogPrefab.gameObject.AddComponent<ButtonRolloverHandler>();
            buttonRolloverHandler.Target = rend;
            buttonRolloverHandler.OutColor = Color.white;
            buttonRolloverHandler.OverColor = Color.gray;
            CogPrefab.gameObject.layer = 5;
            CogPrefab.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        private static void CreateModsPage()
        {
            ModsPagePrefab = new GameObject("ModsPage") { transform = { parent = Parent } }.AddComponent<ModsPage>();
            ModsPagePrefab.gameObject.layer = 5;
            TextMeshPro text = new GameObject("ModsText").AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.characterSpacing = 7;
            text.enableWordWrapping = false;
            text.transform.SetParent(ModsPagePrefab.transform);
            text.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            text.transform.localPosition = new Vector3(0, 1.9f, 0);
            TextMeshPro pageText = new GameObject("PageText").AddComponent<TextMeshPro>();
            pageText.alignment = TextAlignmentOptions.Center;
            pageText.text = "0/10";
            pageText.transform.SetParent(ModsPagePrefab.transform);
            pageText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pageText.transform.localPosition = new Vector3(0, -1.8f, -0.1f);
            PassiveButton rightButton = CreateNextButton("RightButton");
            rightButton.transform.SetParent(ModsPagePrefab.transform);
            rightButton.transform.localPosition = new Vector3(1, -1.8f, -0.1f);
            rightButton.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            PassiveButton leftButton = CreateNextButton("LeftButton");
            leftButton.transform.SetParent(ModsPagePrefab.transform);
            leftButton.transform.localPosition = new Vector3(-1, -1.8f, -0.1f);
            leftButton.transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
            for (int i = 0; i < 10; i++)
            {
                TextMeshPro modsText = new GameObject("ModText " + i.ToString()).AddComponent<TextMeshPro>();
                modsText.alignment = TextAlignmentOptions.Top;
                modsText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                modsText.enableWordWrapping = false;
                modsText.transform.SetParent(ModsPagePrefab.transform);
                modsText.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                modsText.transform.localPosition = new Vector3(0, 1.25f - i * 0.27f, -0.1f);
                modsText.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                ButtonRolloverHandler buttonRolloverHandler = modsText.gameObject.AddComponent<ButtonRolloverHandler>();
                buttonRolloverHandler.TargetText = modsText;
                buttonRolloverHandler.OverColor = Color.cyan;
                modsText.gameObject.AddComponent<PassiveButton>().ClickSound = SelectSound;
            }
        }
        private static void CreatePluginChanger()
        {
            PluginChangerPrefab = new GameObject("PluginChanger") { transform = { parent = Parent } }.AddComponent<PluginChanger>();
            PluginChangerPrefab.gameObject.layer = 5;
            PluginChangerPrefab.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            PluginChangerPrefab.gameObject.AddComponent<SpriteRenderer>().sprite = PluginChangerBackground;
            TextMeshPro text = new GameObject("Text").AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.transform.SetParent(PluginChangerPrefab.transform);
            text.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
            text.gameObject.layer = 5;
            text.enableAutoSizing = true;
            PassiveButton rightButton = CreateNextButton("RightButton");
            rightButton.transform.SetParent(PluginChangerPrefab.transform);
            rightButton.transform.localPosition = new Vector3(4, 0, 0);
            rightButton.transform.localScale = Vector3.one;
            PassiveButton leftButton = CreateNextButton("LeftButton");
            leftButton.transform.SetParent(PluginChangerPrefab.transform);
            leftButton.transform.localScale = new Vector3(-1, 1, 1);
            leftButton.transform.localPosition = new Vector3(-4, 0, 0);
        }
        public static PassiveButton CreateNextButton(string name)
        {
            PassiveButton button = new GameObject(name).AddComponent<PassiveButton>();
            button.ClickSound = SelectSound;
            button.HoverSound = HoverSound;
            BoxCollider2D boxCollider2D = button.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            boxCollider2D.size *= 2;
            SpriteRenderer rend = button.gameObject.AddComponent<SpriteRenderer>();
            rend.sprite = NextButton;
            ButtonRolloverHandler buttonRolloverHandler = button.gameObject.AddComponent<ButtonRolloverHandler>();
            buttonRolloverHandler.Target = rend;
            buttonRolloverHandler.OutColor = Color.white;
            buttonRolloverHandler.OverColor = new Color32(44, 235, 198, byte.MaxValue);
            button.gameObject.layer = 5;
            return button;
        }
    }
}
