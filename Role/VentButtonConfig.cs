using System;
using UnityEngine;

namespace FungleAPI.Role
{
    public class VentButtonConfig
    {
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
        public Func<bool> CanUse;
        public Func<float> Cooldown;
        public Func<bool> ShowOutline;
        public Action<Vent> SetTarget;
        public Action<float> SetTimer;
        public Action DoClick;
        public Action Update;
        public Action ResetButton;
        public Action InitializeButton;
        public float Timer;
        public VentButton Button => HudManager.Instance.ImpostorVentButton;
    }
}
