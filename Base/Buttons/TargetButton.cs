using FungleAPI.Attributes;
using FungleAPI.Hud;
using Rewired.Utils;
using UnityEngine.UI;

namespace FungleAPI.Base.Buttons
{
    /// <summary>
    /// Base class to create a target button
    /// </summary>
    [FungleIgnore]
    public abstract class TargetButton<T> : CustomAbilityButton where T : UnityEngine.Object
    {
        /// <summary>
        /// Returns the current target
        /// </summary>
        public T Target;
        public override bool CanUse() => base.CanUse() && Target != null;
        /// <summary>
        /// Activates or deactivates the outline of the given target
        /// </summary>
        public virtual void SetOutline(T target, bool active) { }
        /// <summary>
        /// Get a target
        /// </summary>
        public virtual T GetTarget()
        {
            return default;
        }
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
