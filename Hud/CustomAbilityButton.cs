using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace FungleAPI.Hud
{
    [FungleIgnore]
    public abstract class CustomAbilityButton
    {
        internal static Dictionary<Type, CustomAbilityButton> Buttons = new Dictionary<Type, CustomAbilityButton>();
        public static T Instance<T>() where T : CustomAbilityButton
        {
            CustomAbilityButton button;
            Buttons.TryGetValue(typeof(T), out button);
            return button.SimpleCast<T>();
        }
        public abstract string OverrideText { get; }
        public abstract Color32 TextOutlineColor { get; }
        public abstract Sprite ButtonSprite { get; }
        public abstract float Cooldown { get; }
        public virtual float InitialCooldown => Cooldown / 2f;
        public virtual ButtonLocation Location => ButtonLocation.BottomRight;
        public virtual int MaxUses { get; }
        public virtual bool Active { get; }
        public virtual bool CanCooldown => true;
        public virtual bool LimitedUses => MaxUses > 0;
        public virtual bool TransformButton { get; }
        public virtual float TransformDuration => 0f;
        public virtual bool CanDetransform => true;
        public AbilityButton Button { get; protected set; }
        public float Timer { get; protected set; }
        public float TransformTimer { get; protected set; }
        public int UsesLeft { get; protected set; }
        public bool Transformed { get; protected set; }
        public abstract void OnClick();
        public virtual void CreateButton()
        {
            if (Button)
            {
                return;
            }
            Reset(ResetType.Create);
            Button = GameObject.Instantiate(HudManager.Instance.AbilityButton, Location == ButtonLocation.BottomRight ? HudHelper.BottomRight : HudHelper.BottomLeft);
            Button.name = OverrideText;
            Button.graphic.sprite = ButtonSprite;
            Button.OverrideText(OverrideText);
            Button.buttonLabelText.SetOutlineColor(TextOutlineColor);
            if (LimitedUses)
            {
                Button.SetUsesRemaining(MaxUses);
            }
            else
            {
                Button.usesRemainingSprite.gameObject.SetActive(false);
            }
            Button.GetComponent<PassiveButton>().SetNewAction(ClickHandler);
        }
        public virtual bool CanUse()
        {
            return !Minigame.Instance && !MeetingHud.Instance && Vent.currentVent == null && (Transformed && CanDetransform || !Transformed);
        }
        public virtual void MeetingStart(MeetingHud meetingHud) 
        {
            if (Transformed)
            {
                EndTransform();
            }
        }
        public virtual void Enable() { }
        public virtual void Reset(ResetType resetType)
        {
            Timer = resetType == ResetType.Create ? InitialCooldown : Cooldown;
            UsesLeft = resetType != ResetType.EndMeeting ? MaxUses : UsesLeft;
            Transformed = false;
            TransformTimer = TransformDuration;
        }
        public virtual bool CanClick()
        {
            return CanUse() && (Timer <= 0f) && (!LimitedUses || UsesLeft > 0);
        }
        public virtual void SetCooldown(float cooldown) 
        {
            Timer = cooldown; 
            Button.SetCoolDown(Timer, Cooldown); 
        }
        public virtual void SetNumUses(int numUses) 
        { 
            UsesLeft = numUses; 
            Button.SetUsesRemaining(numUses); 
        }
        public virtual void SetTransformDuration(float newDuration) 
        {
            TransformTimer = newDuration;
            Button.SetFillUp(Timer, TransformDuration); 
        }
        public virtual void ClickHandler()
        {
            if (!CanClick())
            {
                return;
            }
            if (TransformButton && Transformed && CanDetransform)
            {
                EndTransform();
                return;
            }
            if (LimitedUses)
            {
                UsesLeft--;
                Button?.SetUsesRemaining(UsesLeft);
            }
            OnClick();
            if (TransformButton)
            {
                Transformed = true;
                TransformTimer = TransformDuration;
            }
            Timer = Cooldown;
        }
        public virtual void Update()
        {
            UpdateTimer();
            UpdateUI();
        }
        protected virtual void UpdateTimer()
        {
            if (MeetingHud.Instance || ExileController.Instance)
            {
                return;
            }
            if (TransformButton && Transformed)
            {
                TransformTimer -= Time.deltaTime;
                Button.SetFillUp(TransformTimer, TransformDuration);
                if (TransformTimer <= 0f)
                {
                    EndTransform();
                }
            }
            else if (CanCooldown && Timer > 0)
            {
                Timer -= Time.deltaTime;
                if (Timer < 0)
                {
                    Timer = 0;
                }
                Button.SetCoolDown(Timer, Cooldown);
            }
        }
        protected virtual void UpdateUI()
        {
            bool enabled = CanUse() && (!LimitedUses || UsesLeft > 0);
            Color color = enabled ? Palette.EnabledColor : Palette.DisabledClear;
            int desat = enabled ? 0 : 1;
            Button.graphic.color = color;
            Button.graphic.material.SetFloat("_Desat", desat);
            Button.buttonLabelText.color = color;
        }
        public virtual void EndTransform()
        {
            Transformed = false;
            TransformTimer = TransformDuration;
        }
        public virtual void Destroy()
        {
            if (!Button)
            {
                return;
            }
            GameObject.Destroy(Button.gameObject);
            Button = null;
        }
        public enum ResetType
        {
            EndMeeting,
            Default,
            Create
        }
    }
}
