using FungleAPI.Role;
using System;

namespace FungleAPI.GameOptions.Groups
{
    public interface IRoleOptionGroup
    {
        Type RoleType { get; }
        string GroupName { get; }
    }

    public abstract class AbstractOptionGroup<TRole> : IRoleOptionGroup where TRole : RoleBehaviour, ICustomRole
    {
        public Type RoleType => typeof(TRole);
        public abstract string GroupName { get; }
    }

    public static class OptionGroupSingleton<TGroup> where TGroup : class, IRoleOptionGroup
    {
        public static TGroup Instance { get; internal set; }
    }
}
