using FungleAPI.Patches;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class CustomDeadBody : MonoBehaviour
    {
        public static List<CustomDeadBody> AllBodies = new List<CustomDeadBody>();
        public PlayerControl Carring;
        public PlayerControl Owner;
        public DeadBody Body;
        public void Destroy()
        {
            AllBodies.Remove(this);
            Destroy(gameObject);
        }
        public void Start()
        {
            Body = GetComponent<DeadBody>();
            if (Body != GameManager.Instance.DeadBodyPrefab)
            {
                Owner = Utils.GetPlayerById(Body.ParentId);
                AllBodies.Add(this);
                GetComponent<BoxCollider2D>().isTrigger = false;
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
            foreach (SpriteRenderer spriteRenderer in body.bodyRenderers)
            {
                from.SetPlayerMaterialColors(spriteRenderer);
            }
            from.SetPlayerMaterialColors(body.bloodSplatter);
            Vector3 vector = from.transform.position + from.KillAnimations[0].BodyOffset;
            vector.z = vector.y / 1000f;
            body.transform.position = vector;
            body.enabled = true;
            CustomDeadBody Cbody = body.gameObject.AddComponent<CustomDeadBody>();
            Cbody.Owner = from;
            Cbody.GetComponent<BoxCollider2D>().isTrigger = false;
            AllBodies.Add(Cbody);
            return Cbody;
        }
    }
}
