using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class PlayerHelper : MonoBehaviour
    {
        public PlayerControl Player => GetComponent<PlayerControl>();
        public bool ToogleCosmetics = true;
        public RoleBehaviour OldRole = RoleManager.Instance.GetRole(AmongUs.GameOptions.RoleTypes.Crewmate);
        public void Update()
        {
            if (!Player.Data.IsDead && !Player.inVent)
            {
                Player.cosmetics.SetBodyCosmeticsVisible(ToogleCosmetics);
                Player.cosmetics.TogglePetVisible(ToogleCosmetics);
                Player.cosmetics.ToggleNameVisible(ToogleCosmetics);
            }
        }
    }
}
