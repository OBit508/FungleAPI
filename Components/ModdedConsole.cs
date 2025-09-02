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
            bool flag = true;
            if (pc.IsDead)
            {
                flag = DeadsCanUse;
            }
            PlayerControl @object = pc.Object;
            Vector3 center = @object.Collider.bounds.center;
            Vector3 position = transform.position;
            float num = Vector2.Distance(center, position);
            canUse = num <= UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
            couldUse = flag;
            return num;
        }
        public override void Use()
        {
            SystemConsole c = HudManager.Instance.UseButton.currentTarget.SafeCast<SystemConsole>();
            if (c != null && (Vector2.Distance(PlayerControl.LocalPlayer.transform.position, transform.position) <= UsableDistance || c == this) && !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, PlayerControl.LocalPlayer.Collider.bounds.center, transform.position, Constants.ShipOnlyMask, false) && !Minigame.Instance)
            {
                bool flag = true;
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    flag = DeadsCanUse;
                }
                if (flag)
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
        public bool DeadsCanUse;
        public Color OutlineColor;
        public static ModdedConsole CreateConsole(float distance, bool deadsCanUse, Action onUse, Sprite sprite)
        {
            ModdedConsole console = new GameObject("CustomConsole").AddComponent<ModdedConsole>();
            console.gameObject.layer = 12;
            console.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
            console.Image = console.gameObject.AddComponent<SpriteRenderer>();
            console.gameObject.AddComponent<PassiveButton>().SetNewAction(console.Use);
            console.MinigamePrefab = RoleManager.Instance.AllRoles.FirstOrDefault(obj => obj.Role == AmongUs.GameOptions.RoleTypes.Scientist).SafeCast<ScientistRole>().VitalsPrefab;
            console.Image.material = new Material(Shader.Find("Sprites/Outline"));
            console.DeadsCanUse = deadsCanUse;
            console.OnUse = onUse;
            console.Image.sprite = sprite;
            console.usableDistance = distance;
            console.transform.localScale = Vector3.one;
            console.transform.position = Vector3.zero;
            return console;
        }
    }
}
