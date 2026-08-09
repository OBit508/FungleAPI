using FungleAPI.Event;
using FungleAPI.Event.Vanilla.Player;
using FungleAPI.Modifiers;
using System;
using System.Collections.Generic;

namespace FungleAPI.Components
{
    public class ModifierHolder : PlayerComponent
    {
        public static ModifierHolder LocalPlayer;
        public Dictionary<uint, BaseModifier> Modifiers = new Dictionary<uint, BaseModifier>();
        private readonly List<BaseModifier> iterationBuffer = new List<BaseModifier>();
        public void Update()
        {
            iterationBuffer.Clear();
            iterationBuffer.AddRange(Modifiers.Values);
            foreach (BaseModifier baseModifier in iterationBuffer)
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
            iterationBuffer.Clear();
            iterationBuffer.AddRange(Modifiers.Values);
            foreach (BaseModifier baseModifier in iterationBuffer)
            {
                baseModifier.FixedUpdate();
            }
        }
        public void CallOnDeath(DeathReason reason)
        {
            iterationBuffer.Clear();
            iterationBuffer.AddRange(Modifiers.Values);
            foreach (BaseModifier baseModifier in iterationBuffer)
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
            iterationBuffer.Clear();
            iterationBuffer.AddRange(Modifiers.Values);
            foreach (BaseModifier modifier in iterationBuffer)
            {
                modifier.Deinitialize();
            }
            iterationBuffer.Clear();
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
