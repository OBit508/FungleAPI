using FungleAPI.Attributes;
using FungleAPI.Hud;
using Rewired.Utils;

namespace FungleAPI.Base.Buttons
{
    [FungleIgnore]
    public class TargetButton<T> : CustomAbilityButton where T : UnityEngine.Object
    {
        public T Target;
        public override bool CanUse => Target != null;
        public override bool CanClick => CanUse;
        public virtual void SetOutline(T target, bool active)
        {
        }
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
