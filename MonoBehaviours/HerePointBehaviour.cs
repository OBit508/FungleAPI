using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.MonoBehaviours
{
    public class HerePointBehaviour : MonoBehaviour
    {
        public PlayerControl Player;
        public void FixedUpdate()
        {
            if (Player != null)
            {
                Vector3 vector = Player.transform.position;
                vector /= ShipStatus.Instance.MapScale;
                vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                vector.z = -1f;
                transform.localPosition = vector;
                return;
            }
            GameObject.Destroy(gameObject);
            return;
        }
        public static HerePointBehaviour CreateHerePoint(PlayerControl player)
        {
            SpriteRenderer renderer = GameObject.Instantiate<SpriteRenderer>(MapBehaviour.Instance.HerePoint, MapBehaviour.Instance.HerePoint.transform.parent);
            player.SetPlayerMaterialColors(renderer);
            HerePointBehaviour herePoint = renderer.gameObject.AddComponent<HerePointBehaviour>();
            herePoint.Player = player;
            return herePoint;
        }
    }
}
