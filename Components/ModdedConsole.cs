using Cpp2IL.Core.Attributes;
using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Components
{
    [RegisterTypeInIl2Cpp]
    public class ModdedConsole : SystemConsole
    {
        public override float CanUse(NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
        {
            PlayerControl @object = pc.Object;
            Vector3 center = @object.Collider.bounds.center;
            Vector3 position = transform.position;
            float num = Vector2.Distance(center, position);
            canUse = num <= UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
            couldUse = (Predicate == null || Predicate != null && Predicate(PlayerControl.LocalPlayer));
            return num;
        }
        public override void Use()
        {
            SystemConsole c = HudManager.Instance.UseButton.currentTarget.SafeCast<SystemConsole>();
            if (c != null && (Vector2.Distance(PlayerControl.LocalPlayer.transform.position, transform.position) <= UsableDistance || c == this) && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, PlayerControl.LocalPlayer.Collider.bounds.center, transform.position, Constants.ShipOnlyMask, false) && !Minigame.Instance)
            {
                if (Predicate == null || Predicate != null && Predicate(PlayerControl.LocalPlayer))
                {
                    OnUse?.Invoke();
                }
            }
        }
        public override void SetOutline(bool on, bool mainTarget)
        {
            if (on)
            {
                Image.material.SetFloat("_Outline", 1f);
                Image.material.SetColor("_OutlineColor", OutlineColor);
            }
            else
            {
                Image.material.SetFloat("_Outline", 0f);
            }
            if (mainTarget)
            {
                float num = Mathf.Clamp01(OutlineColor.r * 0.5f);
                float num2 = Mathf.Clamp01(OutlineColor.g * 0.5f);
                float num3 = Mathf.Clamp01(OutlineColor.b * 0.5f);
                Image.material.SetColor("_AddColor", new Color(num, num2, num3, 1f));
            }
            else
            {
                Image.material.SetColor("_AddColor", new Color(0f, 0f, 0f, 0f));
            }
        }
        public Action OnUse;
        public Predicate<PlayerControl> Predicate;
        public Color OutlineColor;
    }
}
