using FungleAPI.Attributes;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FungleAPI.Hud
{
    /// <summary>
    /// Base class to create a custom ability button
    /// </summary>
    [FungleIgnore]
    public abstract class CustomAbilityButton
    {
        /// <summary>
        /// Text displayed on the button
        /// </summary>
        public abstract string OverrideText { get; }
        /// <summary>
        /// Outline color of the button text
        /// </summary>
        public abstract Color32 TextOutlineColor { get; }
        /// <summary>
        /// Sprite used as the button icon
        /// </summary>
        public abstract Sprite ButtonSprite { get; }
        /// <summary>
        /// Cooldown duration after each use
        /// </summary>
        public abstract float Cooldown { get; }
        /// <summary>
        /// Initial cooldown applied when the button is created
        /// </summary>
        public virtual float InitialCooldown => Cooldown / 2f;
        /// <summary>
        /// Screen location where the button will be placed
        /// </summary>
        public virtual ButtonLocation Location => ButtonLocation.BottomRight;
        /// <summary>
        /// Maximum number of uses available
        /// </summary>
        public virtual int MaxUses { get; }
        /// <summary>
        /// Whether the button is currently active
        /// </summary>
        public virtual bool Active { get; }
        /// <summary>
        /// Whether the button can enter cooldown
        /// </summary>
        public virtual bool CanCooldown => true;
        /// <summary>
        /// Indicates if the button has limited uses
        /// </summary>
        public virtual bool LimitedUses => MaxUses > 0;
        /// <summary>
        /// Whether the button supports transform behavior
        /// </summary>
        public virtual bool TransformButton { get; }
        /// <summary>
        /// Duration of the transform state
        /// </summary>
        public virtual float TransformDuration => 0f;
        /// <summary>
        /// Whether the button can exit transform state manually
        /// </summary>
        public virtual bool CanDetransform => true;
        /// <summary>
        /// The underlying AbilityButton instance
        /// </summary>
        public AbilityButton Button { get; protected set; }
        /// <summary>
        /// Current cooldown timer value
        /// </summary>
        public float Timer { get; protected set; }
        /// <summary>
        /// Current transform timer value
        /// </summary>
        public float TransformTimer { get; protected set; }
        /// <summary>
        /// Remaining number of uses
        /// </summary>
        public int UsesLeft { get; protected set; }
        /// <summary>
        /// Whether the button is currently transformed
        /// </summary>
        public bool Transformed { get; protected set; }
        /// <summary>
        /// Called when the button is clicked
        /// </summary>
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
        /// <summary>
        /// Reset types used to control reset behavior
        /// </summary>
        public enum ResetType
        {
            EndMeeting,
            Default,
            Create
        }
    }
}