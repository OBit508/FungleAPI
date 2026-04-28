using FungleAPI.Components;
using FungleAPI.GameMode.Patches;
using FungleAPI.ModCompatibility;
using FungleAPI.Utilities.Prefabs;
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
        /// <summary>
        /// Returns the ship type
        /// </summary>
        public static ShipType GetShipType(this ShipStatus shipStatus)
        {
            if (LevelImpostorSupport.LevelImpostorAssembly != null && shipStatus.GetComponent(Il2CppType.From(LevelImpostorSupport.LIShipStatus)) != null)
            {
                return ShipType.LevelImpostor;
            }
            if (SubmergedSupport.SubmergedAssembly != null && shipStatus.GetComponent(Il2CppType.From(SubmergedSupport.SubmarineStatus)) != null)
            {
                return ShipType.Submerged;
            }
            Type type = shipStatus.GetType();
            if (type == typeof(SkeldShipStatus))
            {
                return ShipType.Skeld;
            }
            else if (type == typeof(MiraShipStatus))
            {
                return ShipType.MiraHQ;
            }
            else if (type == typeof(PolusShipStatus))
            {
                return ShipType.Polus;
            }
            else if (type == typeof(AirshipStatus))
            {
                return ShipType.Airship;
            }
            return ShipType.Fungle;
        }

        /// <summary>
        /// Create a vent
        /// </summary>
        public static Vent CreateVent(this ShipStatus shipStatus, VentType type, Vector2 position, List<Vent> nearbyVents = null, bool connectBoth = true)
        {
            Vent prefab = null;
            switch (type)
            {
                case VentType.Skeld:
                    prefab = PrefabUtils.SkeldPrefab.AllVents[0]; break;
                case VentType.Polus:
                    prefab = PrefabUtils.PolusPrefab.AllVents[0]; break;
                case VentType.Fungle:
                    prefab = PrefabUtils.FunglePrefab.AllVents[0]; break;
            }
            if (prefab == null && !VentExtensions.VentPrefabs.TryGetValue(type, out prefab))
            {
                FungleAPIPlugin.Instance.Log.LogError($"Failed to create a vent with type {type}, the prefab cant be found.");
                return null;
            }
            Vent vent = GameObject.Instantiate<Vent>(prefab, shipStatus.transform);
            vent.gameObject.SetActive(true);
            vent.Id = shipStatus.AllVents.Count;
            shipStatus.AllVents = shipStatus.AllVents.Concat(new Vent[] { vent }).ToArray();
            vent.Right = null;
            vent.Center = null;
            vent.Left = null;
            vent.transform.position = new Vector3(position.x, position.y, position.y / 1000 + 0.001f);
            VentPatch.DoStart(vent);
            vent.TryGetHelper().Vents.AddRange(nearbyVents);
            return vent;
        }
    }
}
