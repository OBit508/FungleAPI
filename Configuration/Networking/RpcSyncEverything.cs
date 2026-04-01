using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Configuration;
using FungleAPI.Configuration.Attributes;
using FungleAPI.Configuration.Helpers;
using FungleAPI.Networking;
using FungleAPI.Patches;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppSystem.Runtime.Remoting.Messaging;
using InnerNet;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Configuration.Networking
{
    internal class RpcSyncEverything : SimpleRpc
    {
        public override void Write(MessageWriter messageWriter)
        {
            Rpc<RpcSyncGameOpt>.Instance.Write(messageWriter, ModPlugin.AllPlugins);
            Rpc<RpcSyncTeam>.Instance.Write(messageWriter, ModdedTeamManager.Teams.Values);
            Rpc<RpcSyncRole>.Instance.Write(messageWriter, CustomRoleManager.AllCustomRoles);
        }
        public override void Handle(MessageReader messageReader)
        {
            try
            {
                Rpc<RpcSyncGameOpt>.Instance.Handle(messageReader);
                Rpc<RpcSyncTeam>.Instance.Handle(messageReader);
                Rpc<RpcSyncRole>.Instance.Handle(messageReader);
            }
            catch (Exception ex)
            {
                AmongUsClient.Instance.HandleDisconnect((DisconnectReasons)244);
                FungleAPIPlugin.Instance.Log.LogError(ex);
            }
        }
    }
}
