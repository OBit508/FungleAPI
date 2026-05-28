using AmongUs.Data;
using Assets.CoreScripts;
using FungleAPI.Attributes;
using FungleAPI.Extensions;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities.Sound;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOver
{
    /// <summary>
    /// Base class to create a custom game over
    /// </summary>
    [FungleIgnore]
    public class BaseGameOver
    {
        public static BaseGameOver CachedGameOver;
        public List<CachedPlayerData> Winners = new List<CachedPlayerData>();
        public virtual string WinText { get; set; }
        public virtual Color BackgroundColor { get; set; }
        public virtual Color NameColor { get; set; }
        public virtual AudioClip Clip { get; set; }
        public virtual bool HasExtraByte { get; }
        public GameOverReason Reason { get; internal set; }
        public virtual void SetData()
        {
            Winners.Clear();
            foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
            {
                if (networkedPlayerInfo.Role.DidWin(Reason))
                {
                    Winners.Add(new CachedPlayerData(networkedPlayerInfo));
                }
                
            }
        }
        public virtual byte GetExtraByte() { return default; }
        public virtual void InterpretExtraByte(byte b) { }
        public virtual void OnSetEverythingUp(EndGameManager endGameManager) 
        {
            AudioClip winClip = Clip;
            if (winClip == null)
            {
                switch (Reason)
                {
                    case GameOverReason.CrewmatesByVote: winClip = endGameManager.CrewStinger; break;
                    case GameOverReason.CrewmatesByTask: winClip = endGameManager.CrewStinger; break;
                    case GameOverReason.HideAndSeek_CrewmatesByTimer: winClip = endGameManager.CrewStinger; break;
                    case GameOverReason.ImpostorDisconnect: winClip = endGameManager.DisconnectStinger; break;
                    default: winClip = endGameManager.ImpostorStinger; break;
                }
            }
            DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
            endGameManager.Navigation.HideButtons();
            if (Reason == GameOverReason.ImpostorDisconnect)
            {
                DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Draws);
            }
            else
            {
                if (EndGameResult.CachedWinners.ToSystemList().Any((h) => h.IsYou))
                {
                    DataManager.Player.Stats.IncrementWinStats(EndGameResult.CachedGameOverReason, (MapNames)GameManager.Instance.LogicOptions.MapId, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                    DestroyableSingleton<AchievementManager>.Instance.SetWinMap(GameManager.Instance.LogicOptions.MapId);
                    CachedPlayerData cachedPlayerData = EndGameResult.CachedWinners.ToSystemList().FirstOrDefault((h) => h.IsYou);
                    if (cachedPlayerData != null)
                    {
                        DestroyableSingleton<UnityTelemetry>.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId, cachedPlayerData.NamePlateId);
                    }
                }
                else
                {
                    DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Losses);
                }
                SoundManagerHelper.PlayDynamicSound("Stinger", winClip, false, new Utilities.Sound.DynamicSound.GetDynamicsFunction(endGameManager.GetStingerVol), SoundManager.Instance.MusicChannel);
            }
            endGameManager.WinText.color = BackgroundColor;
            endGameManager.WinText.text = WinText;
            endGameManager.BackgroundBar.material.SetColor("_Color", BackgroundColor);
            int num = Mathf.CeilToInt(7.5f);
            List<CachedPlayerData> list = Winners.OrderBy(delegate (CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            }).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                CachedPlayerData cachedPlayerData2 = list[i];
                int num2 = i % 2 == 0 ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = i == 0 ? -8 : -1;
                PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate(endGameManager.PlayerPrefab, endGameManager.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + num3 * 0.01f) * 0.9f;
                float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                Vector3 vector = new Vector3(num7, num7, 1f);
                poolablePlayer.transform.localScale = vector;
                if (cachedPlayerData2.IsDead)
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                }
                else
                {
                    poolablePlayer.SetFlipX(i % 2 == 0);
                }
                poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData2.IsDead, true, null, false);
                if (GameManager.Instance.DidHumansWin(EndGameResult.CachedGameOverReason))
                {
                    poolablePlayer.ToggleName(false);
                }
                else
                {
                    poolablePlayer.SetName(cachedPlayerData2.PlayerName, vector.Inv(), NameColor, -15f);
                    Vector3 vector2 = new Vector3(0f, -1.31f, -0.5f);
                    poolablePlayer.SetNamePosition(vector2);
                    if (AprilFoolsMode.ShouldHorseAround() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek)
                    {
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                        poolablePlayer.SetFlipX(false);
                    }
                }
            }
        }
    }
    [FungleIgnore]
    public class BaseGameOver<DataT> : BaseGameOver
    {
        public virtual void ReceiveDataFromRpcEndGame(DataT data) { }
    }
}
