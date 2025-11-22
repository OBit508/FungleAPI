using FungleAPI.Attributes;
using FungleAPI.Patches;
using FungleAPI.PluginLoading;
using FungleAPI.Role.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using xCloud;
using static Sentry.MeasurementUnit;
namespace FungleAPI.Hud
{
    [FungleIgnore]
    public class CustomAbilityButton
    {
        internal static Dictionary<Type, CustomAbilityButton> Buttons = new Dictionary<Type, CustomAbilityButton>();
        public static T Instance<T>() where T : CustomAbilityButton
        {
            CustomAbilityButton button;
            Buttons.TryGetValue(typeof(T), out button);
            return button.SimpleCast<T>();
        }
        public AbilityButton Button;
        public virtual ButtonLocation Location => ButtonLocation.BottomRight;
        public virtual bool Active => false;
        public virtual bool CanClick { get; }
        public virtual bool CanUse { get; }
        public virtual float Cooldown { get; }
        public virtual float InitialCooldown => Cooldown / 2;
        public float Timer;
        public virtual bool HaveUses { get; }
        public virtual int NumUses { get; }
        public int CurrentNumUses;
        public virtual bool TransformButton { get; }
        public virtual float TransformDuration { get; }
        public float TransformTimer;
        public bool Transformed;
        public virtual void Destransform() 
        {
            Transformed = false;
            TransformTimer = TransformDuration;
        }
        public virtual void Click() { }
        public virtual string OverrideText { get { return "Ability Button"; } }
        public virtual Sprite ButtonSprite { get; }
        public virtual Color32 TextOutlineColor { get { return Color.white; } }
        public void SetCooldown(float cooldown)
        {
            Timer = cooldown;
            Button.SetCoolDown(Timer, Cooldown);
        }
        public void SetNumUses(int numUses)
        {
            CurrentNumUses = numUses;
            Button.SetUsesRemaining(numUses);
        }
        public void SetTransformDuration(float newDuration)
        {
            TransformTimer = newDuration;
            Button.SetCoolDown(Timer, TransformDuration);
        }
        public virtual void MeetingStart(MeetingHud meetingHud)
        {
            if (Transformed)
            {
                Destransform();
            }
        }
        public virtual void Update()
        {
            Color color = Palette.DisabledClear;
            int num = 1;
            bool flag = true;
            if (HaveUses)
            {
                flag = CurrentNumUses > 0;
            }

            if (flag && CanUse && !Minigame.Instance && !MeetingHud.Instance && Vent.currentVent == null)
            {
                color = Palette.EnabledColor;
                num = 0;
            }
            Button.graphic.color = color;
            Button.graphic.material.SetFloat("_Desat", num);
            Button.usesRemainingSprite.color = color;
            Button.usesRemainingSprite.material.SetFloat("_Desat", num);
            Button.usesRemainingText.color = color;
            Button.usesRemainingText.material.SetFloat("_Desat", num);
            Button.buttonLabelText.color = color;
            if (!Transformed)
            {
                if (!MeetingHud.Instance && !ExileController.Instance && Timer > 0f)
                {
                    Timer -= Time.deltaTime;
                    Button.SetCoolDown(Timer, Cooldown);
                }
            }
            else if (!MeetingHud.Instance && !ExileController.Instance && TransformTimer > 0f)
            {
                TransformTimer -= Time.deltaTime;
                Button.SetFillUp(TransformTimer, TransformDuration);
                if (TransformTimer <= 0f)
                {
                    TransformTimer = TransformDuration;
                    Transformed = false;
                    Destransform();
                }
            }
        }
        public virtual void Start() { }
        public virtual void Destroy()
        {
           if (Button != null)
            {
                UnityEngine.Object.Destroy(Button.gameObject);
                Button = null;
            }
        }
        public virtual void Reset(bool creating = false)
        {
            Timer = !creating ? Cooldown : TutorialManager.InstanceExists ? 0f : InitialCooldown;
            CurrentNumUses = NumUses;
            Transformed = false;
            TransformTimer = TransformDuration;
        }
        public AbilityButton CreateButton()
        {
            if (Button != null)
            {
                Destroy();
            }
            Reset(true);
            Button = UnityEngine.Object.Instantiate(HudManager.Instance.AbilityButton, Location == ButtonLocation.BottomRight ? HudHelper.BottomRight : HudHelper.BottomLeft);
            Button.name = GetType().Name;
            PassiveButton component = Button.GetComponent<PassiveButton>();
            Button.graphic.sprite = ButtonSprite;
            Button.graphic.color = new Color(1f, 1f, 1f, 1f);
            Button.gameObject.SetActive(true);
            Button.OverrideText(OverrideText);
            Button.buttonLabelText.SetOutlineColor(TextOutlineColor);
            Button.GetComponent<PassiveButton>().SetNewAction(delegate
            {
                if (CanClick && CanUse)
                {
                    bool flag = true;
                    bool flag2 = true;
                    if (TransformButton)
                    {
                        flag = !Transformed;
                    }

                    if (HaveUses)
                    {
                        flag2 = CurrentNumUses > 0;
                    }

                    if (flag && flag2 && Timer <= 0f)
                    {
                        Timer = Cooldown;
                        if (HaveUses)
                        {
                            CurrentNumUses--;
                            if (Button != null)
                            {
                                Button.SetUsesRemaining(CurrentNumUses);
                            }
                        }
                        Click();
                        if (TransformButton)
                        {
                            Transformed = true;
                        }
                    }

                    if (!flag)
                    {
                        Transformed = false;
                        TransformTimer = TransformDuration;
                        Destransform();
                    }
                }
            });
            Button.SetCoolDown(Timer, Cooldown);
            if (HaveUses)
            {
                Button.SetUsesRemaining(NumUses);
            }
            else
            {
                Button.usesRemainingSprite.gameObject.SetActive(false);
            }
            return Button;
        }
    }
}
