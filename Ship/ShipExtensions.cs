using FungleAPI.Api;
using FungleAPI.Components;
using FungleAPI.ModCompatibility;
using FungleAPI.Ship.Patches;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Ship
{
    /// <summary>
    /// Extensions for the ship
    /// </summary>
    public static class ShipExtensions
    {
        private static ShipStatus sp;
        private static ShipType last;

        /// <summary>
        /// Returns the ship type
        /// </summary>
        public static ShipType GetShipType(this ShipStatus shipStatus)
        {
            if (sp != shipStatus)
            {
                if (LevelImpostorSupport.LevelImpostorAssembly != null && shipStatus.GetComponent(Il2CppType.From(LevelImpostorSupport.LIShipStatus)) != null)
                {
                    last = ShipType.LevelImpostor;
                }
                if (SubmergedCompatibility.Instance != null && shipStatus.GetComponent(Il2CppType.From(SubmergedCompatibility.Instance.SubmarineStatus)) != null)
                {
                    last = ShipType.Submerged;
                }
                if (shipStatus.SafeCast<SkeldShipStatus>() != null)
                {
                    last = ShipType.Skeld;
                }
                else if (shipStatus.SafeCast<MiraShipStatus>() != null)
                {
                    last = ShipType.MiraHQ;
                }
                else if (shipStatus.SafeCast<PolusShipStatus>() != null)
                {
                    last = ShipType.Polus;
                }
                else if (shipStatus.SafeCast<AirshipStatus>() != null)
                {
                    last = ShipType.Airship;
                }
                last = ShipType.Fungle;
                sp = shipStatus;
            }

            return last;
        }

        /// <summary>
        /// Create a vent
        /// </summary>
        public static Vent CreateVent(this ShipStatus shipStatus, VentType type, Vector2 position, int ventId, List<Vent> nearbyVents = null)
        {
            if (type == VentType.Polus && ShipPrefabLoader.PolusPrefab == null)
            {
                throw new Exception("Polus ship is not loaded");
            }
            if (type == VentType.Fungle && ShipPrefabLoader.FunglePrefab == null)
            {
                throw new Exception("TheFungle ship is not loaded");
            }
            Vent prefab = null;
            switch (type)
            {
                case VentType.Skeld:
                    prefab = ShipPrefabLoader.SkeldPrefab.GetComponentsInChildren<Vent>()[0]; break;
                case VentType.Polus:
                    prefab = ShipPrefabLoader.PolusPrefab.GetComponentsInChildren<Vent>()[0]; break;
                case VentType.Fungle:
                    prefab = ShipPrefabLoader.FunglePrefab.GetComponentsInChildren<Vent>()[0]; break;
            }
            if (prefab == null && !VentExtensions.VentPrefabs.TryGetValue(type, out prefab))
            {
                FungleApiPlugin.Instance.Log.LogError($"Failed to create a vent with type {type}, the prefab cant be found.");
                return null;
            }
            Vent vent = GameObject.Instantiate<Vent>(prefab, shipStatus.transform);
            vent.gameObject.SetActive(true);
            vent.Id = ventId;
            shipStatus.AllVents = shipStatus.AllVents.Concat(new Vent[] { vent }).ToArray();
            vent.Right = null;
            vent.Center = null;
            vent.Left = null;
            vent.transform.position = new Vector3(position.x, position.y, position.y / 1000 + 0.001f);
            VentPatch.DoStart(vent);
            
            if (nearbyVents != null)
            {
                vent.TryGetHelper().Vents.AddRange(nearbyVents);
            }

            return vent;
        }
    }
}
