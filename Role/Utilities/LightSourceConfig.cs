using AmongUs.GameOptions;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Utilities
{
    /// <summary>
    /// Configuration class for player light source behavior
    /// </summary>
    public class LightSourceConfig
    {
        /// <summary>
        /// Default light source configuration instance
        /// </summary>
        public static LightSourceConfig Default { get; } = new LightSourceConfig();
        public LightSourceConfig()
        {
            CalculateLightRadius = (NetworkedPlayerInfo player, bool airship) =>
            {
                if (airship)
                {
                    AirshipStatus airshipStatus = Ship.SafeCast<AirshipStatus>();
                    float num = airshipStatus.CalculateLightRadius(player);
                    if (player.Role.AffectedByLightAffectors)
                    {
                        foreach (LightAffector lightAffector in airshipStatus.LightAffectors)
                        {
                            if (player.Object && player.Object.Collider.IsTouching(lightAffector.Hitbox))
                            {
                                num *= lightAffector.Multiplier;
                            }
                        }
                    }
                    return num;
                }
                else
                {
                    if (player == null || player.IsDead)
                    {
                        return Ship.MaxLightRadius;
                    }
                    if (player.Role.IsImpostor)
                    {
                        return Ship.MaxLightRadius * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.ImpostorLightMod);
                    }
                    float t = 1f;
                    ISystemType systemType;
                    if (Ship.Systems.TryGetValue(SystemTypes.Electrical, out systemType))
                    {
                        t = systemType.SafeCast<SwitchSystem>().Value / 255f;
                    }
                    return Mathf.Lerp(Ship.MinLightRadius, Ship.MaxLightRadius, t) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                }
            };
            AdjustLighting = delegate (PlayerControl playerControl)
            {
                if (Player != playerControl)
                {
                    return;
                }
                float flashlightSize = 0f;
                if (IsFlashlightEnabled(playerControl))
                {
                    if (playerControl.Data.Role.IsImpostor)
                    {
                        GameOptionsManager.Instance.CurrentGameOptions.TryGetFloat(FloatOptionNames.ImpostorFlashlightSize, out flashlightSize);
                    }
                    else
                    {
                        GameOptionsManager.Instance.CurrentGameOptions.TryGetFloat(FloatOptionNames.CrewmateFlashlightSize, out flashlightSize);
                    }
                }
                playerControl.SetFlashlightInputMethod();
                playerControl.lightSource.SetupLightingForGameplay(IsFlashlightEnabled(playerControl), flashlightSize, playerControl.TargetFlashlight.transform);
            };
            IsFlashlightEnabled = playerControl =>
            {
                if (LobbyBehaviour.Instance != null)
                {
                    return false;
                }
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    return false;
                }
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
                {
                    return false;
                }
                bool flag = false;
                return GameOptionsManager.Instance.CurrentGameOptions.TryGetBool(BoolOptionNames.UseFlashlight, out flag) && flag;
            };
        }
        /// <summary>
        /// Calculates the light radius for a player
        /// </summary>
        public Func<NetworkedPlayerInfo, bool, float> CalculateLightRadius;
        /// <summary>
        /// Adjusts lighting settings for the given player
        /// </summary>
        public Action<PlayerControl> AdjustLighting;
        /// <summary>
        /// Determines whether the flashlight is enabled for the player
        /// </summary>
        public Predicate<PlayerControl> IsFlashlightEnabled;
        /// <summary>
        /// Updates the light source configuration
        /// </summary>
        public Action Update;
        /// <summary>
        /// Gets the local player
        /// </summary>
        public PlayerControl Player => PlayerControl.LocalPlayer;
        /// <summary>
        /// Gets the current ship status instance
        /// </summary>
        public ShipStatus Ship => ShipStatus.Instance;
        /// <summary>
        /// Gets the player's light source component
        /// </summary>
        public LightSource Light => Player.lightSource;
    }
}