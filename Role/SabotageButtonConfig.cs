using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// Configuration class for the Sabotage button behavior
    /// </summary>
    public class SabotageButtonConfig
    {
        /// <summary>
        /// Default sabotage button configuration instance
        /// </summary>
        public static SabotageButtonConfig Default { get; } = new SabotageButtonConfig();
        /// <summary>
        /// Default sprite used by the sabotage button
        /// </summary>
        public static Sprite DefaultSprite;
        public SabotageButtonConfig()
        {
            Cooldown = () => 0;
            CanUse = () => Button.isActiveAndEnabled && !Button.IsOnCooldown;
            Refresh = delegate
            {
                PlayerControl player = PlayerControl.LocalPlayer;
                if (GameManager.Instance == null || player == null)
                {
                    Button.ToggleVisible(false);
                    Button.SetDisabled();
                    return;
                }
                if (player.inVent || !GameManager.Instance.SabotagesEnabled() || player.petting)
                {
                    Button.ToggleVisible(player.Data.Role.CanSabotage() && GameManager.Instance.SabotagesEnabled());
                }
                if (Button.isActiveAndEnabled)
                {
                    if (CanUse())
                    {
                        Button.SetEnabled();
                        return;
                    }
                    Button.SetDisabled();
                }
            };
            SetTimer = delegate (float time)
            {
                float cooldown = Cooldown();
                if (cooldown <= 0)
                {
                    return;
                }
                Timer = Mathf.Clamp(time, 0, cooldown);
                Button.SetCoolDown(Timer, cooldown);
            };
            ResetTimer = delegate (bool doors)
            {
                if (!doors)
                {
                    SetTimer(Cooldown());
                    if (Timer > 0)
                    {
                        MapBehaviour.Instance?.Close();
                    }
                }
            };
            DoClick = delegate
            {
                if (PlayerControl.LocalPlayer.Data.Role.CanSabotage() && CanUse() && !PlayerControl.LocalPlayer.inVent && GameManager.Instance.SabotagesEnabled())
                {
                    HudManager.Instance.ToggleMapVisible(new MapOptions
                    {
                        Mode = MapOptions.Modes.Sabotage
                    });
                }
            };
            ResetButton = delegate
            {
                SetTimer(0);
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().TargetText = StringNames.SabotageLabel;
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().ResetText();
                Button.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);
                Button.graphic.SetCooldownNormalizedUvs();
                Button.graphic.sprite = DefaultSprite;
            };
            Update = delegate
            {
                SetTimer(Timer - Time.deltaTime);
            };
        }
        /// <summary>
        /// Gets the sabotage cooldown duration
        /// </summary>
        public Func<float> Cooldown;
        /// <summary>
        /// Determines whether the sabotage button can be used
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// Sets the sabotage cooldown timer
        /// </summary>
        public Action<float> SetTimer;
        /// <summary>
        /// Resets the sabotage timer
        /// </summary>
        public Action<bool> ResetTimer;
        /// <summary>
        /// Refreshes the sabotage button state
        /// </summary>
        public Action Refresh;
        /// <summary>
        /// Executes the sabotage button action
        /// </summary>
        public Action DoClick;
        /// <summary>
        /// Updates the sabotage button
        /// </summary>
        public Action Update;
        /// <summary>
        /// Resets the sabotage button to its default state
        /// </summary>
        public Action ResetButton;
        /// <summary>
        /// Initializes the sabotage button
        /// </summary>
        public Action InitializeButton;
        /// <summary>
        /// Current sabotage cooldown timer value
        /// </summary>
        public float Timer;
        /// <summary>
        /// Gets the current SabotageButton instance from the Hud
        /// </summary>
        public SabotageButton Button => HudManager.Instance.SabotageButton;
    }
}