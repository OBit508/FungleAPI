using FungleAPI.Components;
using FungleAPI.Configuration.Patches;
using FungleAPI.Utilities.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using xCloud;

namespace FungleAPI.Utilities.Assets
{
    public static class FungleAssets
    {
        public static Sprite Cog;
        public static Sprite Empty;
        public static Sprite NextButton;
        public static Sprite PluginChangerBackground;
        public static Sprite ArrowButton1;
        public static Sprite ArrowButton2;
        public static Sprite CreditsBackground;
        public static Prefab<GameObject> PluginChangerPrefab;
        public static Prefab<GameObject> CreditsPrefab;
        public static void LoadAll()
        {
            Cog = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.cog", 200f);
            Empty = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.empty", 100);
            PluginChangerBackground = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.pluginChangerBackground", 100);
            NextButton = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.nextButton", 100);
            ArrowButton1 = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.arrowButton1", 100);
            ArrowButton2 = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.arrowButton2", 100);
            CreditsBackground = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.creditsBackground", 100);
            CreatePluginChanger();
            CreateCredits();
        }
        private static void CreateCredits()
        {
            CreditsPrefab = new Prefab<GameObject>(new GameObject("Credits"));
            Credits credits = CreditsPrefab.prefab.AddComponent<Credits>();
            credits.gameObject.layer = 5;
            credits.gameObject.AddComponent<SpriteRenderer>().sprite = CreditsBackground;
            PassiveButton arrowButton = new GameObject("ArrowButton").AddComponent<PassiveButton>();
            BoxCollider2D boxCollider2D = arrowButton.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            boxCollider2D.size = new Vector2(2, 4);
            SpriteRenderer arrow1 = new GameObject("Arrow1").AddComponent<SpriteRenderer>();
            arrow1.sprite = ArrowButton1;
            arrow1.transform.SetParent(arrowButton.transform);
            SpriteRenderer arrow2 = new GameObject("Arrow2").AddComponent<SpriteRenderer>();
            arrow2.sprite = ArrowButton2;
            arrow2.transform.SetParent(arrowButton.transform);
            arrowButton.activeSprites = arrow2.gameObject;
            arrowButton.inactiveSprites = arrow1.gameObject;
            arrowButton.gameObject.layer = 5;
            arrowButton.transform.SetParent(credits.transform);
            arrowButton.transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
            arrowButton.transform.localPosition = new Vector3(-2.135f, 1.7f, -0.1f);
            TextMeshPro text = new GameObject("CreditsText").AddComponent<TextMeshPro>();
            text.text = "<font=\"Brook SDF\" material=\"Brook SDF - WhiteOutline\">Créditos:";
            text.alignment = TextAlignmentOptions.Center;
            text.characterSpacing = 7;
            text.enableWordWrapping = false;
            text.transform.SetParent(credits.transform);
            text.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            text.transform.localPosition = new Vector3(0, 1.9f, 0);
            TextMeshPro modsText = new GameObject("ModsText").AddComponent<TextMeshPro>();
            modsText.alignment = TextAlignmentOptions.Top;
            modsText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            modsText.text = "[FungleAPI v0.2.3]\na\na\na\na\na\na\na\na\na";
            modsText.enableWordWrapping = false;
            modsText.transform.SetParent(credits.transform);
            modsText.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            modsText.transform.localPosition = new Vector3(0, 1.25f, -0.1f);
            TextMeshPro pageText = new GameObject("PageText").AddComponent<TextMeshPro>();
            pageText.alignment = TextAlignmentOptions.Center;
            pageText.text = "0/10";
            pageText.transform.SetParent(credits.transform);
            pageText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pageText.transform.localPosition = new Vector3(0, -1.8f, -0.1f);
            PassiveButton rightButton = CreateNextButton("RightButton");
            rightButton.transform.SetParent(credits.transform);
            rightButton.transform.localPosition = new Vector3(1, -1.8f, -0.1f);
            rightButton.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            PassiveButton leftButton = CreateNextButton("LeftButton");
            leftButton.transform.SetParent(credits.transform);
            leftButton.transform.localPosition = new Vector3(-1, -1.8f, -0.1f);
            leftButton.transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
        }
        private static void CreatePluginChanger()
        {
            PluginChangerPrefab = new Prefab<GameObject>(new GameObject("PluginChanger"));
            PluginChanger pluginChanger = PluginChangerPrefab.prefab.AddComponent<PluginChanger>();
            pluginChanger.gameObject.layer = 5;
            pluginChanger.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            pluginChanger.gameObject.AddComponent<SpriteRenderer>().sprite = PluginChangerBackground;
            TextMeshPro text = new GameObject("Text").AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.transform.SetParent(pluginChanger.transform);
            text.transform.localScale = new Vector3(0.28f, 0.28f, 0.28f);
            text.gameObject.layer = 5;
            PassiveButton rightButton = CreateNextButton("RightButton");
            rightButton.transform.SetParent(pluginChanger.transform);
            rightButton.transform.localPosition = new Vector3(4, 0, 0);
            rightButton.transform.localScale = Vector3.one;
            PassiveButton leftButton = CreateNextButton("LeftButton");
            leftButton.transform.SetParent(pluginChanger.transform);
            leftButton.transform.localScale = new Vector3(-1, 1, 1);
            leftButton.transform.localPosition = new Vector3(-4, 0, 0);
        }
        public static PassiveButton CreateNextButton(string name)
        {
            PassiveButton button = new GameObject(name).AddComponent<PassiveButton>();
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
