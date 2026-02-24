using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// Configuration class for the Vent button behavior
    /// </summary>
    public class VentButtonConfig
    {
        /// <summary>
        /// Default vent button configuration instance
        /// </summary>
        public static VentButtonConfig Default { get; } = new VentButtonConfig();
        /// <summary>
        /// Default sprite used by the vent button
        /// </summary>
        public static Sprite DefaultSprite;
        public VentButtonConfig()
        {
            CanUse = () => Button.isActiveAndEnabled && Button.currentTarget != null && !Button.IsOnCooldown && !PlayerControl.LocalPlayer.Data.IsDead;
            Cooldown = () => 0;
            ShowOutline = () => Button.isActiveAndEnabled && !Button.IsOnCooldown && !PlayerControl.LocalPlayer.Data.IsDead;
            SetTarget = delegate (Vent target)
            {
                Button.currentTarget = target;
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
            DoClick = delegate
            {
                if (CanUse())
                {
                    if (PlayerControl.LocalPlayer.inVent && !PlayerControl.LocalPlayer.walkingToVent)
                    {
                        Timer = Cooldown();
                    }
                    Button.currentTarget.Use();
                }
            };
            ResetButton = delegate
            {
                SetTimer(0);
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().TargetText = StringNames.VentLabel;
                Button.buttonLabelText.GetComponent<TextTranslatorTMP>().ResetText();
                Button.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);
                Button.graphic.SetCooldownNormalizedUvs();
                Button.graphic.sprite = DefaultSprite;
            };
            bool enabled = true;
            Update = delegate
            {
                SetTimer(Timer - Time.deltaTime);
                if (enabled != CanUse())
                {
                    enabled = CanUse();
                    if (enabled)
                    {
                        Button.SetEnabled();
                        return;
                    }
                    Button.SetDisabled();
                }
            };
        }
        /// <summary>
        /// Determines whether the vent button can be used
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// Returns the vent cooldown value
        /// </summary>
        public Func<float> Cooldown;
        /// <summary>
        /// Determines whether the vent outline should be shown
        /// </summary>
        public Func<bool> ShowOutline;
        /// <summary>
        /// Sets the current vent target
        /// </summary>
        public Action<Vent> SetTarget;
        /// <summary>
        /// Sets the vent cooldown timer
        /// </summary>
        public Action<float> SetTimer;
        /// <summary>
        /// Executes the vent button action
        /// </summary>
        public Action DoClick;
        /// <summary>
        /// Updates the vent button state
        /// </summary>
        public Action Update;
        /// <summary>
        /// Resets the vent button to its default state
        /// </summary>
        public Action ResetButton;
        /// <summary>
        /// Initializes the vent button
        /// </summary>
        public Action InitializeButton;
        /// <summary>
        /// Current cooldown timer value
        /// </summary>
        public float Timer;
        /// <summary>
        /// Gets the current VentButton instance from the Hud
        /// </summary>
        public VentButton Button => HudManager.Instance.ImpostorVentButton;
    }
}