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
            foreach (BaseModifier baseModifier in Modifiers.Values)
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
            foreach (BaseModifier baseModifier in Modifiers.Values)
            {
                baseModifier.FixedUpdate();
            }
        }
        public void CallOnDeath(DeathReason reason)
        {
            foreach (BaseModifier baseModifier in Modifiers.Values)
            {
                baseModifier.OnDeath(reason);
            }
        }
        public bool AddModifier(uint modifierId)
        {
            if (ModifierManager.Modifiers.TryGetValue(modifierId, out BaseModifier modifier))
            {
                BaseModifier baseModifier = (BaseModifier)Activator.CreateInstance(modifier.GetType());
                baseModifier.ModifierId = modifierId;
                baseModifier.ModifierOptions = modifier.ModifierOptions;
                baseModifier.Initialize(player);

                if (Modifiers.ContainsKey(modifierId))
                {
                    Modifiers[modifierId].Deinitialize();
                }

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
            ModifierManager.Holders.Remove(ModifierManager.Holders.FirstOrDefault(f => f.Value == this).Key);
        }
        [EventRegister]
        public static void OnPlayerDeath(PlayerDieEvent playerDieEvent)
        {
            playerDieEvent.Source?.GetComponent<ModifierHolder>()?.CallOnDeath(playerDieEvent.Reason);
        }
    }
}
