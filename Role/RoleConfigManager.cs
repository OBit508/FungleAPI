using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role
{
    public static class RoleConfigManager
    {
        public static KillButtonConfig KillConfig { get; private set; } = KillButtonConfig.Default;
        public static LightSourceConfig LightConfig { get; private set; } = LightSourceConfig.Default;
        public static MiraRoleTabConfig RoleTabConfig { get; private set; } = new MiraRoleTabConfig();
        public static ReportButtonConfig ReportConfig { get; private set; } = ReportButtonConfig.Default;
        public static SabotageButtonConfig SabotageConfig { get; private set; } = SabotageButtonConfig.Default;
        public static VentButtonConfig VentConfig { get; private set; } = VentButtonConfig.Default;
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
                    if (RoleTabConfig == null)
                    {
                        RoleTabConfig = new MiraRoleTabConfig(customRole);
                    }
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
                RoleTabConfig = new MiraRoleTabConfig
                {
                    __text = role.NiceName,
                    TabNameColor = role.TeamColor
                };
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
