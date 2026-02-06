using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportButtonConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static ReportButtonConfig Default { get; } = new ReportButtonConfig();
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// 
        /// </summary>
        public Action<bool> SetActive;
        /// <summary>
        /// 
        /// </summary>
        public Action DoClick;
        /// <summary>
        /// 
        /// </summary>
        public Action ResetButton;
        /// <summary>
        /// 
        /// </summary>
        public Action InitializeButton;
        /// <summary>
        /// 
        /// </summary>
        public ReportButton Button => HudManager.Instance.ReportButton;
    }
}
