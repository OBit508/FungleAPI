using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public class SabotageButtonConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static SabotageButtonConfig Default { get; } = new SabotageButtonConfig();
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public Func<float> Cooldown;
        /// <summary>
        /// 
        /// </summary>
        public Func<bool> CanUse;
        /// <summary>
        /// 
        /// </summary>
        public Action<float> SetTimer;
        /// <summary>
        /// 
        /// </summary>
        public Action<bool> ResetTimer;
        /// <summary>
        /// 
        /// </summary>
        public Action Refresh;
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
        public SabotageButton Button => HudManager.Instance.SabotageButton;
    }
}
