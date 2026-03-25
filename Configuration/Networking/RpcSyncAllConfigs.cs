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
using FungleAPI.Translation;
using FungleAPI.Utilities;
using Hazel;
using Il2CppInterop.Generator.Extensions;
using Il2CppSystem.Runtime.Remoting.Messaging;
using InnerNet;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Configuration.Networking
{
    /// <summary>
    /// Rpc that sync the current host settings with others players
    /// </summary>
    public class RpcSyncAllConfigs : SimpleRpc
    {
        public override void Write(MessageWriter writer)
        {
            ConfigurationManager.SerializeOptions(writer);
            ConfigurationManager.SerializeRoleOptions(writer);
            ConfigurationManager.SerializeTeamOptions(writer);
        }
        public override void Handle(MessageReader reader)
        {
            try
            {
                ConfigurationManager.DeserializeOptions(reader);
                ConfigurationManager.DeserializeRoleOptions(reader);
                ConfigurationManager.DeserializeTeamOptions(reader);
            }
            catch (Exception ex)
            {
                AmongUsClient.Instance.HandleDisconnect((DisconnectReasons)244);
                FungleAPIPlugin.Instance.Log.LogError(ex);
            }
        }
    }
}
