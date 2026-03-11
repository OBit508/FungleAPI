using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Player
{
    /// <summary>
    /// A dead body utility class
    /// </summary>
    public static class BodyUtils
    {
        internal static Dictionary<DeadBodyType, DeadBody> BodiePrefabs = new Dictionary<DeadBodyType, DeadBody>();
        private static int __lastBodyId = int.MinValue;
        private static List<DeadBody> allDeadBodies = new List<DeadBody>();
        /// <summary>
        /// Returns all the dead bodies
        /// </summary>
        public static List<DeadBody> AllDeadBodies
        {
            get
            {
                allDeadBodies.RemoveAll(body => body == null || body.IsDestroyedOrNull());
                return allDeadBodies;
            }
        }
        /// <summary>
        /// Get a dead body by the Id
        /// </summary>
        public static DeadBody GetBodyById(byte id)
        {
            foreach (DeadBody body in AllDeadBodies)
            {
                if (body.ParentId == id)
                {
                    return body;
                }
            }
            return null;
        }
        /// <summary>
        /// Create a dead body
        /// </summary>
        public static DeadBody CreateDeadBody(PlayerControl from, DeadBodyType deadBodyType)
        {
            DeadBody body = GameObject.Instantiate<DeadBody>(GameManager.Instance.deadBodyPrefab[deadBodyType == DeadBodyType.Normal ? 0 : 1]);
            body.enabled = false;
            body.ParentId = from.PlayerId;
            body.bodyRenderers.ToList().ForEach(delegate (SpriteRenderer b)
            {
                from.SetPlayerMaterialColors(b);
            });
            from.SetPlayerMaterialColors(body.bloodSplatter);
            Vector3 vector = from.transform.position + from.KillAnimations[0].BodyOffset;
            vector.z = vector.y / 1000f;
            body.transform.position = vector;
            body.enabled = true;
            return body;
        }
        /// <summary>
        /// Register a dead body
        /// </summary>
        public static DeadBodyType RegisterBody(DeadBody deadBody)
        {
            DeadBodyType type = (DeadBodyType)__lastBodyId;
            __lastBodyId++;
            BodiePrefabs.Add(type, deadBody);
            return type;
        }
    }
}
