using FungleAPI.Attributes;

namespace FungleAPI.Modifiers
{
    [FungleIgnore]
    public abstract class BaseModifier
    {
        public PlayerControl Player { get; internal set; }
        public uint TypeId { get; internal set; }
        public float RemainingDuration { get; internal set; }
        public abstract StringNames ModifierName;
        public virtual bool Unique => true;
        public virtual float Duration => -1f;
        public virtual bool HideOnUi => false;
        public virtual bool? CanVent => null;
        public virtual bool? CanUseKillButton => null;
        public virtual void OnAdded() { }
        public virtual void OnRemoved() { }
        public virtual void OnUpdated() { }
        public virtual void OnPlayerDied(DeathReason reason) { }
        public virtual void OnMeetingStarted() { }
    }
}
