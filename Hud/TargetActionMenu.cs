using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FungleAPI.Hud
{
    [RegisterTypeInIl2Cpp]
    public sealed class TargetActionMenu : Minigame
    {
        private readonly List<ShapeshifterPanel> _targets = new List<ShapeshifterPanel>();
        private ShapeshifterPanel _panelPrefab;
        private PassiveButton _cancelButton;

        public TargetActionMenu(IntPtr ptr) : base(ptr)
        {
        }

        public static TargetActionMenu Create()
        {
            TargetActionMenu existing = UnityEngine.Object.FindObjectOfType<TargetActionMenu>();
            if (existing != null)
            {
                return existing;
            }

            ShapeshifterRole role = RoleManager.Instance.GetRole(RoleTypes.Shapeshifter).SafeCast<ShapeshifterRole>();
            ShapeshifterMenu source = role.ShapeshifterMenu;
            ShapeshifterMenu clone = UnityEngine.Object.Instantiate(source);
            TargetActionMenu menu = clone.gameObject.AddComponent<TargetActionMenu>();
            menu._panelPrefab = clone.PanelPrefab;
            menu._cancelButton = clone.BackButton.GetComponent<PassiveButton>();
            menu._cancelButton.OnClick.RemoveAllListeners();
            menu._cancelButton.OnClick.AddListener((UnityAction)menu.CloseMenu);
            UnityEngine.Object.DestroyImmediate(clone);
            menu.transform.SetParent(Camera.main.transform, false);
            menu.transform.localPosition = new Vector3(0f, 0f, -50f);
            menu.gameObject.SetActive(false);
            return menu;
        }

        public void Open(Func<PlayerControl, bool> playerFilter, Action<PlayerControl> onSelected)
        {
            CloseTargets();
            gameObject.SetActive(true);
            Minigame.Instance = this;

            PlayerControl[] players = PlayerControl.AllPlayerControls.ToArray()
                .Where(player => player != null && playerFilter(player))
                .ToArray();

            Il2CppSystem.Collections.Generic.List<UiElement> selectableElements = new Il2CppSystem.Collections.Generic.List<UiElement>();
            for (int index = 0; index < players.Length; index++)
            {
                PlayerControl player = players[index];
                ShapeshifterPanel panel = UnityEngine.Object.Instantiate(_panelPrefab, transform);
                int column = index % 2;
                int row = index / 2;
                panel.transform.localPosition = new Vector3(-1.15f + column * 2.3f, 1.5f - row * 0.72f, -1f);
                panel.SetPlayer(index, player.Data, (Il2CppSystem.Action)(() =>
                {
                    onSelected(player);
                    CloseMenu();
                }));
                panel.NameText.color = player.Data.Role.NameColor;
                _targets.Add(panel);
                selectableElements.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(name, _cancelButton, null, selectableElements);
        }

        public void CloseMenu()
        {
            CloseTargets();
            if (Minigame.Instance == this)
            {
                Minigame.Instance = null;
            }
            gameObject.SetActive(false);
        }

        private void CloseTargets()
        {
            foreach (ShapeshifterPanel panel in _targets)
            {
                if (panel != null)
                {
                    UnityEngine.Object.Destroy(panel.gameObject);
                }
            }
            _targets.Clear();
        }

        public override void Begin(PlayerTask task)
        {
        }
    }
}
