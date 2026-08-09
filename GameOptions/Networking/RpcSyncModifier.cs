using FungleAPI.Api;
using FungleAPI.Base.Rpc;
using FungleAPI.GameOptions.Patches;
using FungleAPI.Modifiers;
using FungleAPI.Networking;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcSyncModifier : AdvancedRpc<BaseModifier, PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, BaseModifier data)
        {
            messageWriter.WriteModifier(data);
            messageWriter.Write(data.ModifierOptions.LocalModifierCount.Value);
            messageWriter.Write(data.ModifierOptions.LocalModifierChance.Value);

            if (!RpcSyncGRnT.UnSynced)
            {
                HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{data.ModifierColor.ToTextColor()}{data.ModifierName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{data.ModifierOptions.LocalModifierCount.Value}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{data.ModifierOptions.LocalModifierChance.Value}%</font>.", false);

                if (LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == data.ModifierOptions.Plugin.ModAssembly)
                {
                    LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
                }
            }
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

            BaseModifier baseModifier = messageReader.ReadModifier();
            baseModifier.ModifierOptions.NonHostModifierCount = messageReader.ReadByte();
            baseModifier.ModifierOptions.NonHostModifierChance = messageReader.ReadByte();

            HudManager.Instance.Notifier.SettingsChangeMessageLogic(StringNames.None, $"{SyncManager.MainFont}{baseModifier.ModifierColor.ToTextColor()}{baseModifier.ModifierName.GetString()}</color></font>: " +
                $"{SyncManager.MainFont}{baseModifier.ModifierOptions.NonHostModifierCount}</font>, " +
                $"{FungleTranslation.ChanceText.GetString()}: {SyncManager.MainFont}{baseModifier.ModifierOptions.NonHostModifierChance}%</font>.", !RpcSyncGRnT.UnSynced);

            if (!RpcSyncGRnT.UnSynced && LobbyViewSettingsPanePatch.Tab != null && LobbyViewSettingsPanePatch.Tab.TabAssembly == baseModifier.ModifierOptions.Plugin.ModAssembly)
            {
                LobbyViewSettingsPanePatch.Tab.RefreshViewTab?.Invoke();
            }
        }
    }
}
