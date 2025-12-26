using System;
using UnityEngine;

namespace FungleAPI.Role
{
    public class ReportButtonConfig
    {
        public static Sprite DefaultSprite;
        public ReportButtonConfig()
        {
            CanUse = () => true;
            SetActive = delegate (bool isActive)
            {
                if (GameManager.Instance == null)
                {
                    Button.SetDisabled();
                    Button.ToggleVisible(false);
                    return;
                }
                if (!GameManager.Instance.CanReportBodies())
                {
                    Button.ToggleVisible(false);
                    return;
                }
                if (isActive && CanUse())
                {
                    Button.SetEnabled();
                    return;
                }
                Button.SetDisabled();
            };
            DoClick = delegate
            {
                if (Button.isActiveAndEnabled && CanUse() && GameManager.Instance.CanReportBodies())
                {
                    PlayerControl.LocalPlayer.ReportClosest();
                }
            };
            ResetButton = delegate
            {
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().TargetText = StringNames.ReportLabel;
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().ResetText();
                Button.buttonLabelText.SetOutlineColor(Color.black);
                Button.graphic.sprite = DefaultSprite;
            };
        }
        public Func<bool> CanUse;
        public Action<bool> SetActive;
        public Action DoClick;
        public Action ResetButton;
        public Action InitializeButton;
        public ReportButton Button => HudManager.Instance.ReportButton;
    }
}
