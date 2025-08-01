﻿using FungleAPI.Patches;
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
namespace FungleAPI.Roles
{
    public class CustomAbilityButton
    {
        internal static List<CustomAbilityButton> buttons = new List<CustomAbilityButton>();
        internal static List<CustomAbilityButton> activeButton = new List<CustomAbilityButton>();
        public static T GetInstance<T>() where T : CustomAbilityButton
        {
            foreach (CustomAbilityButton button in buttons)
            {
                if (button.GetType() == typeof(T))
                {
                    return button.SimpleCast<T>();
                }
            }
            return null;
        }
        public virtual bool Active => true;
        public AbilityButton Button;
        public virtual bool CanClick { get; }
        public virtual bool CanUse { get; }
        public virtual float Cooldown { get; }
        public float Timer;
        public virtual bool HaveUses { get; }
        public virtual int NumUses { get; }
        public int CurrentNumUses;
        public virtual bool TransformButton { get; }
        public virtual float TransformDuration { get; }
        public float TransformTimer;
        public bool Transformed;
        public virtual void Destransform() { }
        public virtual void Click() { }
        public virtual string OverrideText { get { return "Ability Button"; } }
        public virtual Sprite ButtonSprite { get; }
        public virtual Color32 TextOutlineColor { get { return Color.white; } }
        public void SetCooldown(float cooldown)
        {
            Timer = cooldown;
            Button.SetCoolDown(Timer, cooldown);
        }
        public void SetNumUses(int numUses)
        {
            CurrentNumUses = numUses;
            Button.SetUsesRemaining(numUses);
        }
        public void SetTransformDuration(float newDuration)
        {
            TransformTimer = newDuration;
            Button.SetCoolDown(Timer, newDuration);
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
            if (Button != null)
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
                if (!Transformed)
                {
                    if (!MeetingHud.Instance && !ExileController.Instance && Vent.currentVent == null && Timer > 0f)
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
            else if (activeButton.Contains(this))
            {
                activeButton.Remove(this);
            }
        }
        public void Destroy()
        {
           if (Button != null)
            {
                activeButton.Remove(this);
                GameObject.Destroy(Button.gameObject);
                Button = null;
            }
        }
        public AbilityButton CreateButton()
        {
            activeButton.Add(this);
            if (Button != null)
            {
                Destroy();
            }
            Timer = Cooldown;
            CurrentNumUses = NumUses;
            Transformed = false;
            TransformTimer = TransformDuration;
            Button = UnityEngine.Object.Instantiate(HudPatch.prefab, DestroyableSingleton<HudManager>.Instance.AbilityButton.transform.parent);
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
                Button.usesRemainingSprite.transform.gameObject.SetActive(false);
            }
            return Button;
        }
        internal static CustomAbilityButton RegisterButton(Type type, ModPlugin plugin)
        {
            try
            {
                CustomAbilityButton button = (CustomAbilityButton)Activator.CreateInstance(type);
                buttons.Add(button);
                plugin.BasePlugin.Log.LogInfo("Registered CustomButton " + type.Name);
                return button;
            }
            catch
            {
                plugin.BasePlugin.Log.LogError("Failed to register Button " + type.Name);
                return null;
            }
        }
    }
}
