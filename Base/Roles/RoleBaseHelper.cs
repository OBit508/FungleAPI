using FungleAPI.Attributes;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Base.Roles
{
    [FungleIgnore]
    public class RoleBaseHelper : RoleBehaviour
    {
        public virtual bool ValidTarget(NetworkedPlayerInfo target)
        {
            return !(target == null) && !target.Disconnected && !target.IsDead && target.PlayerId != this.Player.PlayerId && !(target.Role == null) && !(target.Object == null) && !target.Object.inVent && !target.Object.inMovingPlat && target.Object.Visible;
        }
        public override PlayerControl FindClosestTarget()
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playersInAbilityRangeSorted = GetPlayersInAbilityRangeSorted(GetTempPlayerList().ToSystemList(), false);
            if (playersInAbilityRangeSorted.Count <= 0)
            {
                return null;
            }
            return playersInAbilityRangeSorted[0];
        }
        public Il2CppSystem.Collections.Generic.List<PlayerControl> GetPlayersInAbilityRangeSorted(List<PlayerControl> outputList, bool ignoreColliders)
        {
            outputList.Clear();
            float abilityDistance = GetAbilityDistance();
            Vector2 myPos = Player.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                NetworkedPlayerInfo networkedPlayerInfo = allPlayers[i];
                if (ValidTarget(networkedPlayerInfo))
                {
                    PlayerControl @object = networkedPlayerInfo.Object;
                    if (@object && @object.Collider.enabled)
                    {
                        Vector2 vector = @object.GetTruePosition() - myPos;
                        float magnitude = vector.magnitude;
                        if (magnitude <= abilityDistance && (ignoreColliders || !PhysicsHelpers.AnyNonTriggersBetween(myPos, vector.normalized, magnitude, Constants.ShipAndObjectsMask)))
                        {
                            outputList.Add(@object);
                        }
                    }
                }
            }
            outputList.Sort(delegate (PlayerControl a, PlayerControl b)
            {
                float magnitude2 = (a.GetTruePosition() - myPos).magnitude;
                float magnitude3 = (b.GetTruePosition() - myPos).magnitude;
                if (magnitude2 > magnitude3)
                {
                    return 1;
                }
                if (magnitude2 < magnitude3)
                {
                    return -1;
                }
                return 0;
            });
            return outputList.ToIl2CppList();
        }
        public virtual void AppendHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
        {
            if (this.GetHintType() == RoleHintType.MiraAPI_RoleTab)
            {
                CustomRoleManager.CreateForRole(taskStringBuilder, this);
                return;
            }
            base.AppendTaskHint(taskStringBuilder);
        }
        public override DeadBody FindClosestBody()
        {
            return Player.GetClosestDeadBody(GetAbilityDistance());
        }
    }
}
