using FungleAPI.Attributes;
using UnityEngine;

namespace FungleAPI.Modifiers
{
    [FungleIgnore]
    public abstract class BaseModifier
    {
        public PlayerControl Player { get; internal set; }
        public uint TypeId { get; internal set; }
        public float RemainingDuration { get; internal set; }
        public abstract StringNames ModifierName { get; }
        public virtual Color ModifierColor => Color.gray;
        public virtual bool Unique => true;
        public virtual float Duration => -1f;
        public virtual void OnAdded()
        {
        }
        public virtual void OnRemoved()
        {
        }
        public virtual void OnUpdated()
        {
        }
        public virtual void OnPlayerDied(DeathReason reason)
        {
        }
        public virtual void OnMeetingStarted()
        {
        }
    }
}
