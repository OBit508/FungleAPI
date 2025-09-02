using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FungleAPI.Utilities;
using UnityEngine;
using FungleAPI.Attributes;

namespace FungleAPI.MonoBehaviours
{
    [RegisterTypeInIl2Cpp]
    public class DeadBodyHelper : MonoBehaviour
    {
        public static List<Il2CppSystem.Type> AllBodyComponents = new List<Il2CppSystem.Type>();
        public void Awake()
        {
            DeadBody body = GetComponent<DeadBody>();
            if (GameManager.Instance && GameManager.Instance.DeadBodyPrefab == body || body == null)
            {
                return;
            }
            foreach (Il2CppSystem.Type type in AllBodyComponents)
            {
                gameObject.AddComponent(type);
            }
            Helpers.AllDeadBodies.Add(body);
        }
    }
}
