using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using FungleAPI.Networking;
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

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(EndGameManager), "SetEverythingUp")]
    internal static class EndGameManagerPatch
    {
        public static bool Prefix(EndGameManager __instance)
        {
            if (EndGameHelper.Winners.Count > 0 && EndGameResult.CachedGameOverReason == EndGameHelper.CustomGameOverReason)
            {
                DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
                __instance.Navigation.HideButtons();
                if (EndGameHelper.Winners.Any((CachedPlayerData h) => h.IsYou))
                {
                    DataManager.Player.Stats.IncrementWinStats(EndGameResult.CachedGameOverReason, (MapNames)GameManager.Instance.LogicOptions.MapId, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                    DestroyableSingleton<AchievementManager>.Instance.SetWinMap((int)GameManager.Instance.LogicOptions.MapId);
                    __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Victory);
                    __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                    CachedPlayerData cachedPlayerData = EndGameHelper.Winners.FirstOrDefault((CachedPlayerData h) => h.IsYou);
                    if (cachedPlayerData != null)
                    {
                        DestroyableSingleton<UnityTelemetry>.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId, cachedPlayerData.NamePlateId);
                    }
                }
                else
                {
                    DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Losses);
                    __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Defeat);
                    __instance.WinText.color = Color.red;
                }
                SoundManagerHelper.PlayDynamicSound("Stinger", __instance.ImpostorStinger, false, new Utilities.Sound.DynamicSound.GetDynamicsFunction(__instance.GetStingerVol), SoundManager.Instance.MusicChannel);
                int num = Mathf.CeilToInt(7.5f);
                List<CachedPlayerData> list = EndGameHelper.Winners.OrderBy(delegate (CachedPlayerData b)
                {
                    if (!b.IsYou)
                    {
                        return 0;
                    }
                    return -1;
                }).ToList<CachedPlayerData>();
                for (int i = 0; i < list.Count; i++)
                {
                    CachedPlayerData cachedPlayerData2 = list[i];
                    int num2 = ((i % 2 == 0) ? (-1) : 1);
                    int num3 = (i + 1) / 2;
                    float num4 = (float)num3 / (float)num;
                    float num5 = Mathf.Lerp(1f, 0.75f, num4);
                    float num6 = (float)((i == 0) ? (-8) : (-1));
                    PoolablePlayer poolablePlayer = global::UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                    poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
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
                    Color color = EndGameHelper.NameColor;
                    poolablePlayer.SetName(cachedPlayerData2.PlayerName, vector.Inv(), color, -15f);
                    Vector3 vector2 = new Vector3(0f, -1.31f, -0.5f);
                    poolablePlayer.SetNamePosition(vector2);
                    if (AprilFoolsMode.ShouldHorseAround() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    {
                        poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                        poolablePlayer.SetFlipX(false);
                    }
                }
                EndGameHelper.Winners.Clear();
            }
            else
            {
                DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
                __instance.Navigation.HideButtons();
                bool flag = GameManager.Instance.DidHumansWin(EndGameResult.CachedGameOverReason);
                if (EndGameResult.CachedGameOverReason == GameOverReason.ImpostorDisconnect)
                {
                    DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Draws);
                    __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ImpostorDisconnected);
                    SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false, 1f, null);
                }
                else
                {
                    if (EndGameResult.CachedWinners.ToSystemList().Any((CachedPlayerData h) => h.IsYou))
                    {
                        DataManager.Player.Stats.IncrementWinStats(EndGameResult.CachedGameOverReason, (MapNames)GameManager.Instance.LogicOptions.MapId, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                        DestroyableSingleton<AchievementManager>.Instance.SetWinMap((int)GameManager.Instance.LogicOptions.MapId);
                        __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Victory);
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        CachedPlayerData cachedPlayerData = EndGameResult.CachedWinners.ToSystemList().FirstOrDefault((CachedPlayerData h) => h.IsYou);
                        if (cachedPlayerData != null)
                        {
                            DestroyableSingleton<UnityTelemetry>.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId, cachedPlayerData.NamePlateId);
                        }
                    }
                    else
                    {
                        DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Losses);
                        __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Defeat);
                        __instance.WinText.color = Color.red;
                    }
                    if (flag)
                    {
                        SoundManagerHelper.PlayDynamicSound("Stinger", __instance.CrewStinger, false, new Utilities.Sound.DynamicSound.GetDynamicsFunction(__instance.GetStingerVol), SoundManager.Instance.MusicChannel);
                    }
                    else
                    {
                        SoundManagerHelper.PlayDynamicSound("Stinger", __instance.ImpostorStinger, false, new Utilities.Sound.DynamicSound.GetDynamicsFunction(__instance.GetStingerVol), SoundManager.Instance.MusicChannel);
                    }
                }
                int num = Mathf.CeilToInt(7.5f);
                List<CachedPlayerData> list = EndGameResult.CachedWinners.ToSystemList().OrderBy(delegate (CachedPlayerData b)
                {
                    if (!b.IsYou)
                    {
                        return 0;
                    }
                    return -1;
                }).ToList<CachedPlayerData>();
                for (int i = 0; i < list.Count; i++)
                {
                    CachedPlayerData cachedPlayerData2 = list[i];
                    int num2 = ((i % 2 == 0) ? (-1) : 1);
                    int num3 = (i + 1) / 2;
                    float num4 = (float)num3 / (float)num;
                    float num5 = Mathf.Lerp(1f, 0.75f, num4);
                    float num6 = (float)((i == 0) ? (-8) : (-1));
                    PoolablePlayer poolablePlayer = global::UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                    poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
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
                    if (flag)
                    {
                        poolablePlayer.ToggleName(false);
                    }
                    else
                    {
                        Color color = ModdedTeam.Teams.FirstOrDefault(t => t.WinReason.Contains(EndGameResult.CachedGameOverReason)).TeamColor;
                        poolablePlayer.SetName(cachedPlayerData2.PlayerName, vector.Inv(), color, -15f);
                        Vector3 vector2 = new Vector3(0f, -1.31f, -0.5f);
                        poolablePlayer.SetNamePosition(vector2);
                        if (AprilFoolsMode.ShouldHorseAround() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                        {
                            poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                            poolablePlayer.SetFlipX(false);
                        }
                    }
                }
            }
            return false;
        }
    }
}
