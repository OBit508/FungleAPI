using FungleAPI.Attributes;
using FungleAPI.Translation;

namespace FungleAPI.Modifiers
{
    [FungleIgnore]
    public abstract class BaseModifier
    {
        public PlayerControl Player { get; internal set; }
        public uint TypeId { get; internal set; }
        public float RemainingDuration { get; internal set; }
        public virtual StringNames ModifierName => TranslationManager.GetStringName(GetType().Name);
        public string NiceName
        {
            get
            {
                if (TranslationManager.Translators.TryGetValue(ModifierName, out var translator))
                    return translator.GetString();
                if (TranslationController.InstanceExists)
                    return TranslationController.Instance.GetString(ModifierName);
                return GetType().Name;
            }
        }
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
