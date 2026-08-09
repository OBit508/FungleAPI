using FungleAPI.Attributes;
using FungleAPI.Components;
using FungleAPI.ModCompatibility.MiraSupport;
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
            Sprite sprite = ButtonSprite;
            if (sprite == null || HudManager.Instance?.AbilityButton == null)
            {
                FungleAPI.Api.FungleApiPlugin.Instance.Log.LogError($"Failed to create button {GetType().Name}: button sprite or HUD template is missing");
                return;
            }
            Button = GameObject.Instantiate(HudManager.Instance.AbilityButton, Location == ButtonLocation.BottomRight ? HudHelper.BottomRight : HudHelper.BottomLeft);
            Button.name = OverrideText;
            Button.graphic.sprite = sprite;
            Button.OverrideText(OverrideText);
            Button.buttonLabelText.SetOutlineColor(TextOutlineColor);
            SetCooldown(InitialCooldown);
            if (!LimitedUses)
            {
                Button.usesRemainingSprite.gameObject.SetActive(false);
            }
            Button.GetComponent<PassiveButton>().SetNewAction(ClickHandler);

            if (MiraCompatibility.Instance != null)
            {
                Button.transform.Find("KeybindIcon")?.gameObject.SetActive(false);
            }
        }
        public virtual bool CanUse()
        {
            return Minigame.Instance == null && MeetingHud.Instance == null && Vent.currentVent == null && (Transformed && CanDetransform || !Transformed);
        }
        public virtual void MeetingStart(MeetingHud meetingHud)
        {
            if (Transformed)
            {
                EndTransform();
            }
        }
        public virtual void Enable() { }
        public virtual void Reset(bool changeRole = false)
        {
            if (Button == null) return;

            if (changeRole && LimitedUses)
            {
                SetNumUses(MaxUses);
            }
            SetCooldown(changeRole ? (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay ? 0.01f : InitialCooldown) : Cooldown);
            Transformed = false;
            TransformTimer = TransformDuration;
        }
        public virtual bool CanClick()
        {
            return Active &&
                   HudHelper.Active &&
                   Button != null &&
                   Button.graphic != null &&
                   Button.graphic.enabled &&
                   CanUse() &&
                   Timer <= 0f &&
                   (!LimitedUses || UsesLeft > 0);
        }
        public virtual void SetCooldown(float cooldown)
        {
            Timer = cooldown;
            Button?.SetCoolDown(Timer, Cooldown);
        }
        public virtual void SetNumUses(int numUses)
        {
            UsesLeft = numUses;
            Button?.SetUsesRemaining(numUses);
        }
        public virtual void SetTransformDuration(float newDuration)
        {
            TransformTimer = newDuration;
            Button?.SetFillUp(Timer, TransformDuration);
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
            if (!Button)
                return;
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
            Color color = enabled ? Palette.EnabledColor : new Color(1f, 1f, 1f, 0.5f);
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
    }
}
