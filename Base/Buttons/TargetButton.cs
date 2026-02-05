using FungleAPI.Attributes;
using FungleAPI.Hud;
using Rewired.Utils;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// 
    /// </summary>
    [FungleIgnore]
    public abstract class TargetButton<T> : CustomAbilityButton where T : UnityEngine.Object
    {
        /// <summary>
        /// 
        /// </summary>
        public T Target;
        /// <summary>
        /// 
        /// </summary>
        public override bool CanUse() => base.CanUse() && Target != null;
        /// <summary>
        /// 
        /// </summary>
        public virtual void SetOutline(T target, bool active) { }
        /// <summary>
        /// 
        /// </summary>
        public virtual T GetTarget()
        {
            return default;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            base.Update();
            T newTarget = GetTarget();
            if (newTarget != Target)
            {
                if (Target != null && !Target.IsNullOrDestroyed())
                {
                    SetOutline(Target, false);
                }
                if (newTarget != null && !newTarget.IsNullOrDestroyed())
                {
                    SetOutline(newTarget, true);
                }
                Target = newTarget;
            }
        }
    }
}
