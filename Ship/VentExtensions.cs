using FungleAPI.Components;
using FungleAPI.GameMode.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Ship
{
    /// <summary>
    /// Extensions for the vents
    /// </summary>
    public static class VentExtensions
    {
        private static int __lastVentId = int.MinValue;
        internal static Dictionary<VentType, Vent> VentPrefabs = new Dictionary<VentType, Vent>();
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
