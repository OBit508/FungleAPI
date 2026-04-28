using FungleAPI.Attributes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameMode
{
    /// <summary>
    /// Base class to create a custom game mode
    /// </summary>
    [FungleIgnore]
    public abstract class CustomGameMode
    {
        public int GameModeId;
        public DefaultOptionCollection OptionCollection;
        public abstract StringNames GameModeName { get; }
        public virtual void FixedUpdate() { }
        public virtual MapOptions GetMapOptions() => null;
        public virtual DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole) => null;
        public virtual bool CanUse(IUsable usable, PlayerControl player) => false;
        public virtual void CheckEndCriteria(GameManager gameManager) { }
        public virtual void AssignTasks(ShipStatus shipStatus) { }
        public virtual void SelectRoles(RoleManager roleManager) { }
        public virtual float CanUseVent(Vent vent, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse) { canUse = false; couldUse = false; return 0; }
    }
}
