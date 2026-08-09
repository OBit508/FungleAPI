using FungleAPI.Event;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Components
{
    public class ModifierHolder : PlayerComponent
    {
        public static ModifierHolder LocalPlayer;
        public Dictionary<uint, BaseModifier> Modifiers = new Dictionary<uint, BaseModifier>();
        public void Update()
        {
            foreach (BaseModifier baseModifier in Modifiers.Values.ToArray())
            {
                baseModifier.Update();
            }
            if (LocalPlayer == null && player.AmOwner)
            {
                LocalPlayer = this;
            }
        }
        public void FixedUpdate()
        {
            foreach (BaseModifier baseModifier in Modifiers.Values.ToArray())
            {
                baseModifier.FixedUpdate();
            }
        }
        public void CallOnDeath(DeathReason reason)
        {
            foreach (BaseModifier baseModifier in Modifiers.Values.ToArray())
            {
                baseModifier.OnDeath(reason);
            }
        }
        public bool AddModifier(uint modifierId)
        {
            if (ModifierManager.Modifiers.TryGetValue(modifierId, out BaseModifier modifier))
            {
                if (Modifiers.TryGetValue(modifierId, out BaseModifier currentModifier))
                {
                    currentModifier.Deinitialize();
                }

                BaseModifier baseModifier = (BaseModifier)Activator.CreateInstance(modifier.GetType());
                baseModifier.ModifierId = modifierId;
                baseModifier.ModifierOptions = modifier.ModifierOptions;
                baseModifier.Initialize(player);

                Modifiers[modifierId] = baseModifier;
                return true;
            }
            return false;
        }
        public bool RemoveModifier(uint modifierId)
        {
            if (Modifiers.TryGetValue(modifierId, out BaseModifier baseModifier))
            {
                baseModifier.Deinitialize();
                Modifiers.Remove(modifierId);
                return true;
            }
            return false;
        }
        private void OnDestroy()
        {
            foreach (BaseModifier modifier in Modifiers.Values.ToArray())
            {
                modifier.Deinitialize();
            }
            Modifiers.Clear();

            if (player != null)
            {
                ModifierManager.Holders.Remove(player);
            }
            if (LocalPlayer == this)
            {
                LocalPlayer = null;
            }
        }
        [EventRegister]
        public static void OnPlayerDeath(PlayerDieEvent playerDieEvent)
        {
            playerDieEvent.Source?.GetComponent<ModifierHolder>()?.CallOnDeath(playerDieEvent.Reason);
        }
    }
}
