using FungleAPI.Attributes;
using FungleAPI.Role;
using FungleAPI.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    public class VentHelper : VentComponent
    {
        public static Dictionary<Vent, VentHelper> ShipVents = new Dictionary<Vent, VentHelper>();
        internal ButtonBehavior ArrowPrefab;
        public List<Vent> Vents = new List<Vent>();
        public void Start()
        {
            List<ButtonBehavior> buttons = new List<ButtonBehavior>();
            List<GameObject> cleaningIndicators = new List<GameObject>();
            foreach (Vent vent in Vents)
            {
                ButtonBehavior button = GameObject.Instantiate<ButtonBehavior>(ArrowPrefab, transform);
                buttons.Add(button);
                cleaningIndicators.Add(button.transform.GetChild(0).gameObject);
                button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                button.OnClick.AddListener(new Action(delegate
                {
                    string errorText;
                    if (!this.vent.TryMoveToVent(vent, out errorText))
                    {
                        FungleAPIPlugin.Instance.Log.LogError("Local Player failed to move to " + vent.name + " because of " + errorText);
                    }
                }));
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
    }
}
