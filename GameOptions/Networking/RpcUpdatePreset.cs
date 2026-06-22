using AmongUs.GameOptions;
using Epic.OnlineServices;
using FungleAPI.AntiCheat;
using FungleAPI.Base.Rpc;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using xCloud;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.GameOptions.Networking
{
    internal class RpcUpdatePreset : AdvancedRpc<(RulesPresets preset, ModPlugin modPlugin), PlayerControl>
    {
        public override void Write(MessageWriter messageWriter, (RulesPresets preset, ModPlugin modPlugin) value)
        {
            messageWriter.Write((byte)value.preset);
            messageWriter.WritePlugin(value.modPlugin);

            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, DestroyableSingleton<TranslationController>.Instance.GetString(GameOptionsManager.Instance.CurrentGameOptions.GetRulesPresetTitle()), false, RoleTypes.Crewmate);
        }
        public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
        {
            if (innerNetObject == null) return;

            if (AntiCheatManager.Active && !innerNetObject.AmOwner)
            {
                AntiCheatManager.CheaterFinded(innerNetObject.Data.ClientId);

                return;
            }

            RulesPresets rulesPresets = (RulesPresets)messageReader.ReadByte();
            ModPlugin modPlugin = messageReader.ReadPlugin();
            IFungleBasePlugin fungleBasePlugin = modPlugin as IFungleBasePlugin;
            if (fungleBasePlugin != null)
            {
                fungleBasePlugin.SetPreset(rulesPresets, modPlugin);
            }

            HudManager.Instance.Notifier.AddSettingsChangeMessage(StringNames.ModeLabel, DestroyableSingleton<TranslationController>.Instance.GetString(GameOptionsManager.Instance.CurrentGameOptions.GetRulesPresetTitle()), false, RoleTypes.Crewmate);
        }
    }
}
