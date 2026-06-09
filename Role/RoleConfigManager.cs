using FungleAPI.Role.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    /// <summary>
    /// A class that helps the role config system to work
    /// </summary>
    public static class RoleConfigManager
    {
        /// <summary>
        /// Current kill button configuration
        /// </summary>
        public static KillButtonConfig KillConfig { get; private set; } = KillButtonConfig.Default;
        /// <summary>
        /// Current light source configuration
        /// </summary>
        public static LightSourceConfig LightConfig { get; private set; } = LightSourceConfig.Default;
        /// <summary>
        /// Current role tab configuration
        /// </summary>
        public static PlayerTabConfig PlayerTabConfig { get; private set; } = new PlayerTabConfig();
        /// <summary>
        /// Current report button configuration
        /// </summary>
        public static ReportButtonConfig ReportConfig { get; private set; } = ReportButtonConfig.Default;
        /// <summary>
        /// Current sabotage button configuration
        /// </summary>
        public static SabotageButtonConfig SabotageConfig { get; private set; } = SabotageButtonConfig.Default;
        /// <summary>
        /// Current vent button configuration
        /// </summary>
        public static VentButtonConfig VentConfig { get; private set; } = VentButtonConfig.Default;
        /// <summary>
        /// Updates all configurations based on the provided role
        /// </summary>
        public static void UpdateByRole(RoleBehaviour role)
        {
            if (role == null)
            {
                return;
            }
            KillConfig = KillButtonConfig.Default;
            LightConfig = LightSourceConfig.Default;
            PlayerTabConfig = PlayerTabConfig.Default;
            ReportConfig = ReportButtonConfig.Default;
            SabotageConfig = SabotageButtonConfig.Default;
            VentConfig = VentButtonConfig.Default;
            if (role != null)
            {
                ICustomRole customRole = role.CustomRole();
                if (customRole != null)
                {
                    KillConfig = customRole.KillConfig;
                    LightConfig = customRole.LightConfig;
                    PlayerTabConfig = customRole.PlayerTabConfig;
                    ReportConfig = customRole.ReportConfig;
                    SabotageConfig = customRole.SabotageConfig;
                    VentConfig = customRole.VentConfig;
                }
            }
        }
    }
}