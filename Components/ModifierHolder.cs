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

        private readonly List<BaseModifier> modifierList = new List<BaseModifier>();
        private BaseModifier[] iterationBuffer = Array.Empty<BaseModifier>();
        private bool iterationDirty = true;
        private void RebuildIterationBuffer()
        {
            if (!iterationDirty) return;

            if (iterationBuffer.Length != modifierList.Count)
            {
                iterationBuffer = new BaseModifier[modifierList.Count];
            }
                
            modifierList.CopyTo(iterationBuffer);
            iterationDirty = false;
        }
        public void Update()
        {
            RebuildIterationBuffer();

            for (int i = 0; i < iterationBuffer.Length; i++)
            {
                iterationBuffer[i].Update();
            }

            if (LocalPlayer == null && player.AmOwner)
            {
                LocalPlayer = this;
            }
        }
        public void FixedUpdate()
        {
            RebuildIterationBuffer();

            for (int i = 0; i < iterationBuffer.Length; i++)
            {
                iterationBuffer[i].FixedUpdate();
            }
        }
        public void CallOnDeath(DeathReason reason)
        {
            RebuildIterationBuffer();

            for (int i = 0; i < iterationBuffer.Length; i++)
            {
                iterationBuffer[i].OnDeath(reason);
            }
        }
        public bool AddModifier(uint modifierId)
        {
            if (ModifierManager.Modifiers.TryGetValue(modifierId, out BaseModifier modifier))
            {
                if (Modifiers.TryGetValue(modifierId, out BaseModifier currentModifier))
                {
                    currentModifier.Deinitialize();
                    modifierList.Remove(currentModifier);
                }

                BaseModifier baseModifier = (BaseModifier)Activator.CreateInstance(modifier.GetType());
                baseModifier.ModifierId = modifierId;
                baseModifier.ModifierOptions = modifier.ModifierOptions;
                baseModifier.Initialize(player);

                Modifiers[modifierId] = baseModifier;
                modifierList.Add(baseModifier);
                iterationDirty = true;
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
                modifierList.Remove(baseModifier);
                iterationDirty = true;
                return true;
            }
            return false;
        }
        private void OnDestroy()
        {
            for (int i = 0; i < modifierList.Count; i++)
            {
                modifierList[i].Deinitialize();
            }
            if (player != null)
            {
                ModifierManager.Holders.Remove(player);
            }
        }
        [EventRegister]
        public static void OnPlayerDeath(PlayerDieEvent playerDieEvent)
        {
            playerDieEvent.Source?.GetComponent<ModifierHolder>()?.CallOnDeath(playerDieEvent.Reason);
        }
    }
}