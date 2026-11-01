using AmongUs.Data;
using FungleAPI.Api;
using FungleAPI.Event;
using FungleAPI.Event.Vanilla;
using FungleAPI.GameOver.Ends;
using FungleAPI.GameOver.Networking;
using FungleAPI.Networking;
using FungleAPI.Player.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using xCloud;

namespace FungleAPI.GameOver
{
    /// <summary>
    /// Manage the game overs
    /// </summary>
    public static class GameOverManager
    {
        public static Dictionary<Type ,BaseGameOver> CustomGameOvers = new Dictionary<Type, BaseGameOver>();
        internal static ushort GameOverId = 0;
        internal static Dictionary<Type, GameOverReason> VanillaGameOvers = new Dictionary<Type, GameOverReason>()
        {
            { typeof(CrewmateDisconnect), GameOverReason.CrewmateDisconnect },
            { typeof(CrewmatesByTask), GameOverReason.CrewmatesByTask },
            { typeof(CrewmatesByVote), GameOverReason.CrewmatesByVote },
            { typeof(HideAndSeek_CrewmatesByTimer), GameOverReason.HideAndSeek_CrewmatesByTimer },
            { typeof(HideAndSeek_ImpostorsByKills), GameOverReason.HideAndSeek_ImpostorsByKills },
            { typeof(ImpostorDisconnect), GameOverReason.ImpostorDisconnect },
            { typeof(ImpostorsByKill), GameOverReason.ImpostorsByKill },
            { typeof(ImpostorsBySabotage), GameOverReason.ImpostorsBySabotage },
            { typeof(ImpostorsByVote), GameOverReason.ImpostorsByVote },
        };
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static T GetGameOverInstance<T>() where T : BaseGameOver
        {
            return GetGameOverInstance(typeof(T)).SimpleCast<T>() ?? null;
        }
        /// <summary>
        /// Returns the instance of the given type
        /// </summary>
        public static BaseGameOver GetGameOverInstance(Type type)
        {
            if (CustomGameOvers.TryGetValue(type, out BaseGameOver customGameOver))
            {
                return customGameOver;
            }
            return null;
        }
        /// <summary>
        /// Returns the instance of the given GameOverReason
        /// </summary>
        public static BaseGameOver GetGameOver(this ushort gameOverId)
        {
            return CustomGameOvers.Values.FirstOrDefault(g => g.GameOverId == gameOverId);
        }
        public static void RpcEndGame<T>(this GameManager gameManager) where T : BaseGameOver
        {
            RpcEndGame(gameManager, GetGameOverInstance<T>());
        }
        public static void RpcEndGame<T, DataT>(this GameManager gameManager, DataT data) where T : BaseGameOver<DataT>
        {
            RpcEndGame(gameManager, GetGameOverInstance<T>(), data);
        }
        public static void RpcEndGame(this GameManager gameManager, BaseGameOver gameOver)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            if (!amongUsClient.AmHost)
            {
                return;
            }
            if (EventManager.CallEvent(new BeforeGameOver(gameOver)).Cancelled)
            {
                return;
            }
            Rpc<RpcSendEndData>.Instance.Send((gameOver, null), PlayerControl.LocalPlayer);

            CallOver(gameManager);
        }
        public static void RpcEndGame<DataT>(this GameManager gameManager, BaseGameOver<DataT> gameOver, DataT data)
        {
            AmongUsClient amongUsClient = AmongUsClient.Instance;
            if (!amongUsClient.AmHost)
            {
                return;
            }
            if (EventManager.CallEvent(new BeforeGameOver(gameOver)).Cancelled)
            {
                return;
            }
            Rpc<RpcSendEndData>.Instance.Send((gameOver, data), PlayerControl.LocalPlayer);

            CallOver(gameManager);
        }
        public static void RegisterGameOver(Type type, ModPlugin modPlugin)
        {
            BaseGameOver gameOver = (BaseGameOver)Activator.CreateInstance(type);
            if (VanillaGameOvers != null && VanillaGameOvers.TryGetValue(type, out GameOverReason gameOverReason))
            {
                gameOver.GameOverId = (ushort)gameOverReason;
                VanillaGameOvers.Remove(type);
                if (VanillaGameOvers.Count <= 0)
                {
                    VanillaGameOvers = null;
                }
            }
            else
            {
                gameOver.GameOverId = GameOverId;
            }
            GameOverId++;
            modPlugin.BasePlugin.Log.LogInfo("Registered GameOver " + type.Name + " Id: " + ((int)gameOver.GameOverId).ToString());
            CustomGameOvers.Add(type, gameOver);
        }

        private static void CallOver(GameManager gameManager)
        {
            gameManager.ShouldCheckForGameEnd = false;
            gameManager.logger.Info(string.Format("Endgame for {0}", GameOverReason.CrewmatesByTask), null);
            MessageWriter messageWriter = AmongUsClient.Instance.StartEndGame();
            messageWriter.Write((byte)GameOverReason.CrewmatesByTask);
            messageWriter.Write(false);
            AmongUsClient.Instance.FinishEndGame(messageWriter);
        }
    }
}
