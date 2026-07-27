using BepInEx.Configuration;
using FungleAPI.Api;
using FungleAPI.GameOptions.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements.UIR;

namespace FungleAPI.GameOptions.Collections
{
    public class RoleOptionCollection : OptionCollection
    {
        public ConfigEntry<byte> LocalRoleCount;
        public byte NonHostRoleCount;

        public ConfigEntry<byte> LocalRoleChance;
        public byte NonHostRoleChance;

        public ICustomRole Role;

        public void SetLocal(byte count, byte chance)
        {
            LocalRoleCount.Value = count;
            LocalRoleChance.Value = chance;
        }
        public override void LoadOptions(ConfigFile configFile, string collectionId)
        {
            LocalRoleCount = configFile.Bind(collectionId, "RoleCount", Role.Configuration.DefaultCount);
            LocalRoleChance = configFile.Bind(collectionId, "RoleChance", Role.Configuration.DefaultChance);
            base.LoadOptions(configFile, collectionId);
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost && LocalRoleCount != null && LocalRoleChance != null)
            {
                LocalRoleCount.Value = Role.Configuration.DefaultCount;
                LocalRoleChance.Value = Role.Configuration.DefaultChance;
            }
            else
            {
                NonHostRoleCount = Role.Configuration.DefaultCount;
                NonHostRoleChance = Role.Configuration.DefaultChance;
            }

            base.SetAsDefault(amHost);
        }
        public RoleOptionCollection(ICustomRole customRole)
            :base("Roles", customRole.GetType())
        {
            Role = customRole;
        }
    }
}
