using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public class VentButtonConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static VentButtonConfig Default { get; } = new VentButtonConfig();
        /// <summary>
        /// 
        /// </summary>
        public static Sprite DefaultSprite;
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// 
        /// </summary>
        public Func<float> Cooldown;
        /// <summary>
        /// 
        /// </summary>
        public Func<bool> ShowOutline;
        /// <summary>
        /// 
        /// </summary>
        public Action<Vent> SetTarget;
        /// <summary>
        /// 
        /// </summary>
        public Action<float> SetTimer;
        /// <summary>
        /// 
        /// </summary>
        public Action DoClick;
        /// <summary>
        /// 
        /// </summary>
        public Action Update;
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
        public float Timer;
        /// <summary>
        /// 
        /// </summary>
        public VentButton Button => HudManager.Instance.ImpostorVentButton;
    }
}
