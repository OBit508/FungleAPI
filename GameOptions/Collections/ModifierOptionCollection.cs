using BepInEx.Configuration;
using FungleAPI.Modifiers;
using FungleAPI.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Collections
{
    public class ModifierOptionCollection : OptionCollection
    {
        public ConfigEntry<byte> LocalModifierCount;
        public byte NonHostModifierCount;

        public ConfigEntry<byte> LocalModifierChance;
        public byte NonHostModifierChance;

        public BaseModifier Modifier;

        public void SetLocal(byte count, byte chance)
        {
            LocalModifierCount.Value = count;
            LocalModifierChance.Value = chance;
        }
        public override void LoadOptions(ConfigFile configFile, string collectionId)
        {
            LocalModifierCount = configFile.Bind(collectionId, "ModifierCount", Modifier.DefaultCount);
            LocalModifierChance = configFile.Bind(collectionId, "ModifierChance", Modifier.DefaultChance);
            base.LoadOptions(configFile, collectionId);
        }
        public override void SetAsDefault(bool amHost)
        {
            if (amHost && LocalModifierCount != null && LocalModifierChance != null)
            {
                LocalModifierCount.Value = Modifier.DefaultCount;
                LocalModifierChance.Value = Modifier.DefaultChance;
            }
            else
            {
                NonHostModifierCount = Modifier.DefaultCount;
                NonHostModifierChance = Modifier.DefaultChance;
            }

            base.SetAsDefault(amHost);
        }
        public ModifierOptionCollection(BaseModifier baseModifier)
            : base("Modifiers", baseModifier.GetType())
        {
            Modifier = baseModifier;
        }
    }
}
