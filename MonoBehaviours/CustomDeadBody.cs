using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class CustomDeadBody : DeadBody
    {
        public CustomDeadBody(IntPtr ptr) : base(ptr) { }
        internal static List<Il2CppSystem.Type> AllBodyComponents = new List<Il2CppSystem.Type>();
        public static List<CustomDeadBody> AllBodies = new List<CustomDeadBody>();
        public PlayerControl Carring;
        public PlayerControl Owner;
        public void Destroy()
        {
            AllBodies.Remove(this);
            Destroy(gameObject);
        }
        public void Start()
        {
            Owner = Utils.GetPlayerById(ParentId);
            GetComponent<BoxCollider2D>().isTrigger = false;
            if (this != GameManager.Instance.deadBodyPrefab)
            {
                AllBodies.Add(this);
            }
            foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>(true))
            {
                Owner.SetPlayerMaterialColors(rend);
            }
            foreach (Il2CppSystem.Type type in AllBodyComponents)
            {
                gameObject.AddComponent(type);
            }
        }
        public void Update()
        {
            if (Carring != null)
            {
                Vector3 vector = Vector3.Lerp(transform.position, Carring.transform.position, 5 * Time.deltaTime);
                vector.z = vector.y / 1000f;
                transform.position = vector;
            }
        }
        public void ToogleHightLight(bool active, Color color)
        {
            Material material = transform.GetChild(1).GetComponent<SpriteRenderer>().material;
            int num = 0;
            if (active)
            {
                num++;
            }
            material.SetFloat("_Outline", num);
            material.SetColor("_OutlineColor", color);
        }
        public static CustomDeadBody CreateCustomBody(PlayerControl from)
        {
            DeadBody body = Instantiate(GameManager.Instance.deadBodyPrefab);
            body.enabled = false;
            body.ParentId = from.PlayerId;
            Vector3 vector = from.transform.position + from.KillAnimations[0].BodyOffset;
            vector.z = vector.y / 1000f;
            body.transform.position = vector;
            body.enabled = true;
            return body.SafeCast<CustomDeadBody>();
        }
    }
}
