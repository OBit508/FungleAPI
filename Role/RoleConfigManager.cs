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
        public static PlayerTabConfig RoleTabConfig { get; private set; } = new PlayerTabConfig();
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
            KillConfig = null;
            LightConfig = null;
            RoleTabConfig = null;
            ReportConfig = null;
            SabotageConfig = null;
            VentConfig = null;
            if (role != null)
            {
                ICustomRole customRole = role.CustomRole();
                if (customRole != null)
                {
                    KillConfig = customRole.CreateKillConfig();
                    LightConfig = customRole.CreateLightConfig();
                    RoleTabConfig = customRole.CreateRoleTabConfig();
                    ReportConfig = customRole.CreateReportConfig();
                    SabotageConfig = customRole.CreateSabotageConfig();
                    VentConfig = customRole.CreateVentConfig();
                }
            }
            if (KillConfig == null)
            {
                KillConfig = KillButtonConfig.Default;
            }
            if (LightConfig == null)
            {
                LightConfig = LightSourceConfig.Default;
            }
            if (RoleTabConfig == null)
            {
                RoleTabConfig = PlayerTabConfig.Default;
            }
            if (ReportConfig == null)
            {
                ReportConfig = ReportButtonConfig.Default;
            }
            if (SabotageConfig == null)
            {
                SabotageConfig = SabotageButtonConfig.Default;
            }
            if (VentConfig == null)
            {
                VentConfig = VentButtonConfig.Default;
            }
        }
    }
}