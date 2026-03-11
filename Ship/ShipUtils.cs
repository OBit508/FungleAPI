using FungleAPI.Components;
using FungleAPI.ModCompatibility;
using FungleAPI.Patches;
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
    /// A ship utility class
    /// </summary>
    public static class ShipUtils
    {
        private static int __lastVentId = int.MinValue;
        private static Dictionary<VentType, Vent> VentPrefabs = new Dictionary<VentType, Vent>();

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
            if (prefab == null && !VentPrefabs.TryGetValue(type, out prefab))
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

        /// <summary>
        /// Connect this vent with another vent
        /// </summary>
        public static void ConnectVent(this Vent vent, Vent target, bool connectBoth = true)
        {
            VentHelper helper = vent.TryGetHelper();
            if (!helper.Vents.Contains(target))
            {
                helper.Vents.Add(target);
            }
            VentHelper helper2 = target.TryGetHelper();
            if (connectBoth && !helper2.Vents.Contains(vent))
            {
                helper2.Vents.Add(vent);
            }
        }

        /// <summary>
        /// Disconnect this vent with another vent
        /// </summary>
        public static void DisconnectVent(this Vent vent, Vent target, bool disconnectBoth = true)
        {
            VentHelper helper = vent.TryGetHelper();
            if (helper.Vents.Contains(target))
            {
                helper.Vents.Remove(target);
            }
            VentHelper helper2 = target.TryGetHelper();
            if (disconnectBoth && helper2.Vents.Contains(vent))
            {
                helper2.Vents.Remove(vent);
            }
        }

        /// <summary>
        /// Get the VentHelper without errors
        /// </summary>
        public static VentHelper TryGetHelper(this Vent target)
        {
            try
            {
                return VentHelper.ShipVents[target];
            }
            catch
            {
                VentHelper ventHelper = target.GetComponent<VentHelper>();
                if (ventHelper == null)
                {
                    VentPatch.DoStart(target);
                    ventHelper = target.GetComponent<VentHelper>();
                }
                return ventHelper;
            }
        }

        /// <summary>
        /// Register a vent prefab
        /// </summary>
        public static VentType RegisterVent(Vent prefab)
        {
            VentType type = (VentType)__lastVentId;
            __lastVentId++;
            VentPrefabs.Add(type, prefab);
            return type;
        }
    }
}
