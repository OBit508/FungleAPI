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
using UnityEngine.ProBuilder;
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
        public static Sprite CreditsBackground;
        public static Sprite Highlight;
        public static Sprite Folder;
        public static Sprite Inactive;
        public static Sprite XMark;
        public static AudioClip HoverSound;
        public static AudioClip SelectSound;
        public static Prefab<GameObject> PluginChangerPrefab;
        public static Prefab<GameObject> ModsPagePrefab;
        public static Prefab<GameObject> CogPrefab;
        public static Prefab<GameObject> DestroyButtonPrefab;
        public static Prefab<GameObject> SaveButtonPrefab;
        public static void LoadAll()
        {
            Cog = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.cog", 400f);
            Empty = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.empty", 100);
            PluginChangerBackground = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.pluginChangerBackground", 100);
            NextButton = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.nextButton", 100);
            CreditsBackground = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.creditsBackground", 100);
            Highlight = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.highlight", 100);
            Folder = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.folder", 100);
            Inactive = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.inactive", 100);
            XMark = ResourceHelper.LoadSprite(FungleAPIPlugin.Plugin, "FungleAPI.Resources.xMark", 100);
            HoverSound = ResourceHelper.LoadAudio(FungleAPIPlugin.Plugin, "FungleAPI.Resources.UI_Hover", "UI_Hover");
            SelectSound = ResourceHelper.LoadAudio(FungleAPIPlugin.Plugin, "FungleAPI.Resources.UI_Select", "UI_Select");
            CreatePluginChanger();
            CreateModsPage();
            CreateCog();
            CreateDestroyButton();
            CreateFolderButton();
        }
        private static void CreateDestroyButton()
        {
            DestroyButtonPrefab = new Prefab<GameObject>(new GameObject("DestroyButton"));
            PassiveButton button = DestroyButtonPrefab.prefab.AddComponent<PassiveButton>();
            button.ClickSound = SelectSound;
            button.HoverSound = HoverSound;
            BoxCollider2D boxCollider2D = button.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            SpriteRenderer highlight = new GameObject("Highlight").AddComponent<SpriteRenderer>();
            highlight.transform.SetParent(button.transform);
            highlight.sprite = Highlight;
            highlight.transform.localPosition = Vector3.zero;
            highlight.gameObject.layer = 5;
            SpriteRenderer inactive = new GameObject("Inactive").AddComponent<SpriteRenderer>();
            inactive.transform.SetParent(button.transform);
            inactive.sprite = Inactive;
            inactive.transform.localPosition = Vector3.zero;
            inactive.gameObject.layer = 5;
            button.activeSprites = highlight.gameObject;
            button.inactiveSprites = inactive.gameObject;
            SpriteRenderer icon = new GameObject("Icon").AddComponent<SpriteRenderer>();
            icon.transform.SetParent(button.transform);
            icon.transform.localPosition = new Vector3(0, 0, -0.1f);
            icon.sprite = XMark;
            icon.color = Color.gray;
            icon.gameObject.layer = 5;
            ButtonRolloverHandler buttonRolloverHandler = button.gameObject.AddComponent<ButtonRolloverHandler>();
            buttonRolloverHandler.Target = icon;
            buttonRolloverHandler.Target.color = Color.gray;
            buttonRolloverHandler.OutColor = Color.gray;
            buttonRolloverHandler.OverColor = Color.white;
            button.gameObject.layer = 5;
        }
        private static void CreateFolderButton()
        {
            SaveButtonPrefab = new Prefab<GameObject>(new GameObject("SaveButton"));
            PassiveButton button = SaveButtonPrefab.prefab.AddComponent<PassiveButton>();
            button.ClickSound = SelectSound;
            button.HoverSound = HoverSound;
            BoxCollider2D boxCollider2D = button.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            SpriteRenderer highlight = new GameObject("Highlight").AddComponent<SpriteRenderer>();
            highlight.transform.SetParent(button.transform);
            highlight.sprite = Highlight;
            highlight.transform.localPosition = Vector3.zero;
            highlight.gameObject.layer = 5;
            SpriteRenderer inactive = new GameObject("Inactive").AddComponent<SpriteRenderer>();
            inactive.transform.SetParent(button.transform);
            inactive.sprite = Inactive;
            inactive.transform.localPosition = Vector3.zero;
            inactive.gameObject.layer = 5;
            button.activeSprites = highlight.gameObject;
            button.inactiveSprites = inactive.gameObject;
            SpriteRenderer icon = new GameObject("Icon").AddComponent<SpriteRenderer>();
            icon.transform.SetParent(button.transform);
            icon.transform.localPosition = new Vector3(0, 0, -0.1f);
            icon.sprite = Folder;
            icon.color = Color.gray;
            icon.gameObject.layer = 5;
            ButtonRolloverHandler buttonRolloverHandler = button.gameObject.AddComponent<ButtonRolloverHandler>();
            buttonRolloverHandler.Target = icon;
            buttonRolloverHandler.OutColor = Color.gray;
            buttonRolloverHandler.OverColor = Color.white;
            button.gameObject.layer = 5;
        }
        private static void CreateCog()
        {
            CogPrefab = new Prefab<GameObject>(new GameObject("Cog"));
            PassiveButton cog = CogPrefab.prefab.AddComponent<PassiveButton>();
            cog.ClickSound = SelectSound;
            BoxCollider2D boxCollider2D = cog.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            SpriteRenderer rend = cog.gameObject.AddComponent<SpriteRenderer>();
            rend.sprite = Cog;
            rend.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            ButtonRolloverHandler buttonRolloverHandler = cog.gameObject.AddComponent<ButtonRolloverHandler>();
            buttonRolloverHandler.Target = rend;
            buttonRolloverHandler.OutColor = Color.white;
            buttonRolloverHandler.OverColor = Color.gray;
            cog.gameObject.layer = 5;
            cog.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        private static void CreateModsPage()
        {
            ModsPagePrefab = new Prefab<GameObject>(new GameObject("ModsPage"));
            ModsPage page = ModsPagePrefab.prefab.AddComponent<ModsPage>();
            page.gameObject.layer = 5;
            page.gameObject.AddComponent<SpriteRenderer>().sprite = CreditsBackground;
            TextMeshPro text = new GameObject("ModsText").AddComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.characterSpacing = 7;
            text.enableWordWrapping = false;
            text.transform.SetParent(page.transform);
            text.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            text.transform.localPosition = new Vector3(0, 1.9f, 0);
            TextMeshPro pageText = new GameObject("PageText").AddComponent<TextMeshPro>();
            pageText.alignment = TextAlignmentOptions.Center;
            pageText.text = "0/10";
            pageText.transform.SetParent(page.transform);
            pageText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            pageText.transform.localPosition = new Vector3(0, -1.8f, -0.1f);
            PassiveButton rightButton = CreateNextButton("RightButton");
            rightButton.transform.SetParent(page.transform);
            rightButton.transform.localPosition = new Vector3(1, -1.8f, -0.1f);
            rightButton.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            PassiveButton leftButton = CreateNextButton("LeftButton");
            leftButton.transform.SetParent(page.transform);
            leftButton.transform.localPosition = new Vector3(-1, -1.8f, -0.1f);
            leftButton.transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
            for (int i = 0; i < 10; i++)
            {
                TextMeshPro modsText = new GameObject("ModText " + i.ToString()).AddComponent<TextMeshPro>();
                modsText.alignment = TextAlignmentOptions.Top;
                modsText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                modsText.enableWordWrapping = false;
                modsText.transform.SetParent(page.transform);
                modsText.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                modsText.transform.localPosition = new Vector3(0, 1.25f - i * 0.27f, -0.1f);
            }
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
