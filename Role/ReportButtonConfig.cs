using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// Configuration class for the Report button behavior
    /// </summary>
    public class ReportButtonConfig
    {
        /// <summary>
        /// Default report button configuration instance
        /// </summary>
        public static ReportButtonConfig Default { get; } = new ReportButtonConfig();
        /// <summary>
        /// Default sprite used by the report button
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
        /// Determines whether the report button can be used
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// Sets the active state of the report button
        /// </summary>
        public Action<bool> SetActive;
        /// <summary>
        /// Executes the report button action
        /// </summary>
        public Action DoClick;
        /// <summary>
        /// Resets the report button to its default state
        /// </summary>
        public Action ResetButton;
        /// <summary>
        /// Initializes the report button
        /// </summary>
        public Action InitializeButton;
        /// <summary>
        /// Gets the current ReportButton instance from the Hud
        /// </summary>
        public ReportButton Button => HudManager.Instance.ReportButton;
    }
}