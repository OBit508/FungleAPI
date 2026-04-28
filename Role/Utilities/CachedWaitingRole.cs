using AmongUs.GameOptions;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Role.Utilities
{
    internal struct CachedWaitingRole
    {
        public RoleTypes Id;
        public Type Type;
        public ModPlugin Plugin;
        public CachedWaitingRole(RoleTypes id, Type type, ModPlugin modPlugin)
        {
            Id = id;
            Type = type;
            Plugin = modPlugin;
        }
    }
}
