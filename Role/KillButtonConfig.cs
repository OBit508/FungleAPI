using System;
using UnityEngine;

namespace FungleAPI.Role
{
    /// <summary>
    /// 
    /// </summary>
    public class KillButtonConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public static KillButtonConfig Default { get; } = new KillButtonConfig();
        public KillButtonConfig()
        {
            CanUse = () => Button.isActiveAndEnabled && Button.currentTarget != null && !Button.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove;
            Cooldown = () => GameOptionsManager.Instance.CurrentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown);
            CheckClick = delegate (PlayerControl target)
            {
                if (Button.currentTarget && Button.currentTarget == target)
                {
                    DoClick();
                    return;
                }
                if (Button.currentTarget && Button.currentTarget != target && PlayerControl.LocalPlayer.Data.Role.GetPlayersInAbilityRangeSorted(new Il2CppSystem.Collections.Generic.List<PlayerControl>()).Contains(target))
                {
                    SetTarget(target);
                    DoClick();
                }
            };
            SetTarget = delegate (PlayerControl target)
            {
                if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Data.Role)
                {
                    return;
                }
                ICustomRole customRole = PlayerControl.LocalPlayer.Data.Role.CustomRole();
                if (Button.currentTarget && Button.currentTarget != target)
                {
                    Button.currentTarget.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.clear));
                }
                Button.currentTarget = target;
                if (Button.currentTarget)
                {
                    if (customRole != null)
                    {
                        Button.currentTarget.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(customRole.OutlineColor));
                        return;
                    }
                    target.ToggleHighlight(true, PlayerControl.LocalPlayer.Data.Role.TeamType);
                }
            };
            DoClick = delegate
            {
                if (CanUse())
                {
                    PlayerControl.LocalPlayer.CmdCheckMurder(Button.currentTarget);
                    SetTarget(null);
                }
            };
            ResetButton = delegate
            {
                PlayerControl.LocalPlayer?.SetKillTimer(10f);
                Button.ChangeButtonText(StringNames.KillLabel);
                Button.graphic.sprite = Button.defaultKillSprite;
                Button.graphic.SetCooldownNormalizedUvs();
                Button.buttonLabelText.SetOutlineColor(Palette.ImpostorRed);
            };
            bool enabled = true;
            Update = delegate
            {
                if (CanUse() != enabled)
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
        public Action<PlayerControl> CheckClick;
        /// <summary>
        /// 
        /// </summary>
        public Action<PlayerControl> SetTarget;
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
        public KillButton Button => HudManager.Instance.KillButton;
    }
}
