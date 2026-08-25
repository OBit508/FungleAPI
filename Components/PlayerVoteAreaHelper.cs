using AmongUs.Data;
using FungleAPI.Attributes;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class PlayerVoteAreaHelper : MonoBehaviour
    {
        public string NameText;
        public PlayerVoteArea VoteArea;
        public NetworkedPlayerInfo Owner;
        public Action SetColorblindText;
        public void Start()
        {
            NameText = Owner.PlayerName;

            if (PlayerControl.LocalPlayer.Data.Role.CanSeeRole(Owner.Role))
            {
                NameText = $"{Owner.Role.NameColor.ToTextColor()}{Owner.PlayerName}</color>";
                if (Owner.Role.ShowRoleText())
                {
                    NameText += $"\n{Owner.Role.TeamColor.ToTextColor()}<size=60%>{Owner.Role.NiceName}</size></color>";

                    SetColorblindText = () =>
                    {
                        VoteArea.NameText.transform.localPosition = new Vector3(0.3384f, DataManager.Settings.Accessibility.ColorBlindMode ? 0.0911f : 0.0111f, -0.1f);
                    };
                    SetColorblindText();

                    DataManager.Settings.Accessibility.OnColorBlindModeChanged += SetColorblindText;

                    return;
                }
            }
        }
        public void OnDestroy()
        {
            if (SetColorblindText != null)
            {
                DataManager.Settings.Accessibility.OnColorBlindModeChanged -= SetColorblindText;
            }
        }
        public void Update()
        {
            VoteArea.NameText.text = NameText;
        }
    }
}
