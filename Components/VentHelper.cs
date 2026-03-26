using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.Networking;
using FungleAPI.Player;
using FungleAPI.Role;
using FungleAPI.Ship;
using FungleAPI.Utilities;
using FungleAPI.Utilities.Prefabs;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static Rewired.Controller;

namespace FungleAPI.Components
{
    /// <summary>
    /// A component that helps modified ventilation systems to work
    /// </summary>
    public class VentHelper : VentComponent
    {
        internal static Dictionary<Vent, VentHelper> ShipVents = new Dictionary<Vent, VentHelper>();
        public static ButtonBehavior ArrowPrefab;
        public List<Vent> Vents = new List<Vent>();
        public List<PlayerControl> Players = new List<PlayerControl>();
        public void Start()
        {
            foreach (ButtonBehavior buttonBehavior in vent.Buttons)
            {
                GameObject.Destroy(buttonBehavior.gameObject);
            }
            List<ButtonBehavior> buttons = new List<ButtonBehavior>();
            List<GameObject> cleaningIndicators = new List<GameObject>();
            foreach (Vent vent in Vents)
            {
                ButtonBehavior button = GameObject.Instantiate<ButtonBehavior>(ArrowPrefab, transform);
                buttons.Add(button);
                cleaningIndicators.Add(button.transform.GetChild(0).gameObject);
                button.SetNewAction(delegate
                {
                    string errorText;
                    if (!CheckForMoveVent(vent, out errorText))
                    {
                        FungleAPIPlugin.Instance.Log.LogError("Local Player failed to move to " + vent.name + " because of " + errorText);
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.RpcMoveToVent(vent.TryGetHelper());
                    }
                });
            }
            vent.Buttons = buttons.ToArray();
            vent.CleaningIndicators = cleaningIndicators.ToArray();
            vent.SetButtons(Vent.currentVent == vent);
        }
        public void Update()
        {
            if (Vents.Count != vent.Buttons.Count)
            {
                Start();
            }
        }
        /// <summary>
        /// Checks if the local player can move to the given ventilation shaft
        /// </summary>
        public bool CheckForMoveVent(Vent otherVent, out string error)
        {
            if (otherVent == null)
            {
                error = "Vent does not exist";
                return false;
            }
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (!localPlayer.inVent)
            {
                error = "Player is not currently inside a vent";
                return false;
            }
            if (localPlayer.walkingToVent || localPlayer.Visible)
            {
                error = "Player was still in the middle of animating into current vent; not allowed to move vents that fast";
                return false;
            }
            error = "";
            return true;
        }
        /// <summary>
        /// Changes the current ventilation of the given player
        /// </summary>
        public void ChangeCurrentVent(PlayerControl playerControl, Vent otherVent)
        {
            if (Players.Contains(playerControl))
            {
                Players.Remove(playerControl);
            }
            VentHelper other = otherVent.TryGetHelper();
            if (!other.Players.Contains(playerControl))
            {
                other.Players.Add(playerControl);
            }
            playerControl.GetComponent<PlayerHelper>().__CurrentVent = otherVent;
        }
        /// <summary>
        /// Move the player from the current vent to another
        /// </summary>
        public void MoveToVent(PlayerControl playerControl, Vent otherVent)
        {
            ChangeCurrentVent(playerControl, otherVent);
            Vector3 vector = otherVent.transform.position;
            vector -= (Vector3)playerControl.Collider.offset;
            playerControl.NetTransform.SnapTo(vector);
            if (playerControl.AmOwner)
            {
                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(ShipStatus.Instance.VentMoveSounds[UnityEngine.Random.RandomRangeInt(0, ShipStatus.Instance.VentMoveSounds.Count - 1)], false, 1f, null).pitch = FloatRange.Next(0.8f, 1.2f);
                }
                vent.SetButtons(false);
                otherVent.SetButtons(true);
                Vent.currentVent = otherVent;
                VentilationSystem.Update(VentilationSystem.Operation.Move, Vent.currentVent.Id);
            }
            EventManager.CallEvent(new AfterMoveVent(vent, otherVent, playerControl));
        }
    }
}
