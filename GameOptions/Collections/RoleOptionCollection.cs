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
        public const int RoleOptionVersion = 3;

        public int LocalRoleCount;
        public int NonHostRoleCount;

        public int LocalRoleChance;
        public int NonHostRoleChance;

        public ICustomRole Role;

        public void SetLocal(int count, int chance)
        {
            if (LocalRoleCount != count) { LocalRoleCount = count; Dirty = true; }
            if (LocalRoleChance != chance) { LocalRoleChance = chance; Dirty = true; }
        }
        public override void WriteLocalOptions(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(RoleOptionVersion);

            binaryWriter.Write(LocalRoleCount);
            binaryWriter.Write(LocalRoleChance);

            base.WriteLocalOptions(binaryWriter);
        }
        public override void ReadLocalOptions(BinaryReader binaryReader)
        {
            try
            {
                int roleOptionVersion = binaryReader.ReadInt32();
                if (roleOptionVersion < RoleOptionVersion)
                {
                    FungleApiPlugin.Instance.Log.LogWarning($"Different version of the Role Option Collection from {FilePath} founded, loading default.");
                    SetAsDefault(true);
                    return;
                }

                LocalRoleCount = binaryReader.ReadInt32();
                LocalRoleChance = binaryReader.ReadInt32();

                base.ReadLocalOptions(binaryReader);
            }
            catch (Exception ex)
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to read Role Option Collection from {FilePath}, loading default.\nMessage: {ex.Message}");
                SetAsDefault(true);
            }
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost)
            {
                LocalRoleCount = Role.Configuration.DefaultCount;
                LocalRoleChance = Role.Configuration.DefaultChance;
            }
            else
            {
                NonHostRoleCount = Role.Configuration.DefaultCount;
                NonHostRoleChance = Role.Configuration.DefaultChance;
            }

            base.SetAsDefault(amHost);
        }
        public RoleOptionCollection(ICustomRole customRole)
        {
            Role = customRole;
            FolderName = "Roles";
        }
    }
}
