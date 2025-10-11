using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using FungleAPI.Utilities.Sound;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.Utilities;
using UnityEngine.Audio;
using FungleAPI.Role.Teams;
using FungleAPI.GameOver;

namespace FungleAPI.GameOver.Patches
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    internal static class EndGameManagerPatch
    {
        public static bool Prefix(EndGameManager __instance)
        {
            CustomGameOver gameOver = CustomGameOver.CachedGameOver;
            AudioClip winClip = gameOver.Clip;
            if (winClip == null)
            {
                switch (gameOver.Reason)
                {
                    case GameOverReason.CrewmatesByVote: winClip = __instance.CrewStinger; break;
                    case GameOverReason.CrewmatesByTask: winClip = __instance.CrewStinger; break;
                    case GameOverReason.HideAndSeek_CrewmatesByTimer: winClip = __instance.CrewStinger; break;
                    case GameOverReason.ImpostorDisconnect: winClip = __instance.DisconnectStinger; break;
                    default: winClip = __instance.ImpostorStinger; break;
                }
            }
            DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
            __instance.Navigation.HideButtons();
            if (gameOver.Reason == GameOverReason.ImpostorDisconnect)
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
                SoundManagerHelper.PlayDynamicSound("Stinger", winClip, false, new Utilities.Sound.DynamicSound.GetDynamicsFunction(__instance.GetStingerVol), SoundManager.Instance.MusicChannel);
            }
            __instance.WinText.color = gameOver.BackgroundColor;
            __instance.WinText.text = gameOver.WinText;
            __instance.BackgroundBar.material.SetColor("_Color", gameOver.BackgroundColor);
            int num = Mathf.CeilToInt(7.5f);
            List<CachedPlayerData> list = EndGameResult.CachedWinners.OrderBy(delegate (CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            }).ToSystemList();
            for (int i = 0; i < list.Count; i++)
            {
                CachedPlayerData cachedPlayerData2 = list[i];
                int num2 = i % 2 == 0 ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = i == 0 ? -8 : -1;
                PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
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
                    poolablePlayer.SetName(cachedPlayerData2.PlayerName, vector.Inv(), gameOver.NameColor, -15f);
                    Vector3 vector2 = new Vector3(0f, -1.31f, -0.5f);
                    poolablePlayer.SetNamePosition(vector2);
                    if (AprilFoolsMode.ShouldHorseAround() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    {
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                        poolablePlayer.SetFlipX(false);
                    }
                }
            }
            gameOver.OnSetEverythingUp(__instance);
            return false;
        }
    }
}
