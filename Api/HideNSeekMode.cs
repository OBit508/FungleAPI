using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Options;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Api
{
    public class HideNSeekMode : BaseGameMode
    {
        public override StringNames GameModeName => StringNames.GameTypeHideAndSeek;
        public override GameModeOptions ModeOptions { get; } = new HNSOptions();
        public Dictionary<StringNames, IModdedOption> Settings = new Dictionary<StringNames, IModdedOption>();

        public bool IsFinalCountdown
        {
            get
            {
                return this.currentHideTime <= 0f;
            }
        }
        public float CurrentFinalHideTime
        {
            get
            {
                return this.currentFinalHideTime;
            }
        }
        public float TotalFinalHideTime
        {
            get
            {
                return this.totalFinalHideTime;
            }
        }


        private float currentHideTime = float.MaxValue;
        private float currentFinalHideTime = float.MaxValue;
        private float totalFinalHideTime = float.MaxValue;
        private float totalHideTime = float.MaxValue;
        private HideAndSeekTimerBar timerBar;
        private Coroutine beepCoroutine;

        public override void OnGameEnd()
        {
            if (this.timerBar != null)
            {
                GameObject.Destroy(this.timerBar.gameObject);
            }
            if (this.beepCoroutine != null)
            {
                this.Manager.StopCoroutine(this.beepCoroutine);
            }
            this.beepCoroutine = null;
        }
        public override void OnGameStart()
        {
            this.totalHideTime = this.hideAndSeekManager.LogicOptionsHnS.GetEscapeTime();
            this.currentHideTime = this.totalHideTime;
            this.totalFinalHideTime = this.hideAndSeekManager.LogicOptionsHnS.GetFinalCountdownTime();
            this.currentFinalHideTime = this.totalFinalHideTime;
            if (this.timerBar != null)
            {
                GameObject.Destroy(this.timerBar);
            }
            this.timerBar = GameObject.Instantiate<HideAndSeekTimerBar>(GameManagerCreator.Instance.HideAndSeekManagerPrefab.TimerBarPrefab, DestroyableSingleton<HudManager>.Instance.transform.parent);
        }
        public float GetSpeedMod()
        {
            if (Settings[StringNames.GamePlayerSpeed] is ModdedNumberOption moddedNumberOption)
            {
                return moddedNumberOption.FloatValue;
            }
            return 1;
        }
        public override float GetPlayerSpeedMod(PlayerControl pc)
        {
            if (pc == null || pc.Data == null || pc.Data.Role == null)
            {
                return base.GetPlayerSpeedMod(pc);
            }
            if (pc.Data.IsDead)
            {
                return GetSpeedMod() + 1f;
            }
            if (!pc.Data.Role.IsImpostor)
            {
                return GetSpeedMod();
            }
            float num = GetSpeedMod() + GetSpeedMod() * 0.25f;
            if (IsFinalCountdown && Settings[StringNames.SeekerFinalSpeed] is ModdedNumberOption moddedNumber)
            {
                num *= moddedNumber.FloatValue;
            }
            return num;
        }

        // Token: 0x0600122C RID: 4652 RVA: 0x00049637 File Offset: 0x00047837
        public override float GetKillDistance()
        {
            return HideNSeekGameOptionsV10.KillDistances[Mathf.Clamp(0, 0, HideNSeekGameOptionsV10.KillDistances.Length - 1)];
        }

        // Token: 0x0600122D RID: 4653 RVA: 0x00049659 File Offset: 0x00047859
        public override float GetEngineerCooldown()
        {
            return this.GetCrewmateVentCooldown();
        }

        // Token: 0x0600122E RID: 4654 RVA: 0x00049661 File Offset: 0x00047861
        public override float GetEngineerInVentTime()
        {
            return this.GetCrewmateInVentTime();
        }

        // Token: 0x0600122F RID: 4655 RVA: 0x00049669 File Offset: 0x00047869
        public int GetCrewmateLeadTime()
        {
            return 10;
        }

        // Token: 0x06001230 RID: 4656 RVA: 0x0004966D File Offset: 0x0004786D
        public float GetEscapeTime()
        {
            if (Settings[StringNames.EscapeTime] is ModdedNumberOption moddedNumberOption)
            {
                return moddedNumberOption.FloatValue;
            }
            return 200;
        }

        // Token: 0x06001231 RID: 4657 RVA: 0x0004967A File Offset: 0x0004787A
        public float GetFinalCountdownTime()
        {
            if (Settings[StringNames.FinalEscapeTime] is ModdedNumberOption moddedNumberOption)
            {
                return moddedNumberOption.FloatValue;
            }
            return 50;
        }

        // Token: 0x06001232 RID: 4658 RVA: 0x00049687 File Offset: 0x00047887
        public int GetCrewmateVentUses()
        {
            if (Settings[StringNames.MaxVentUses] is ModdedNumberOption moddedNumberOption)
            {
                return moddedNumberOption.IntValue;
            }
            return 1;
        }

        // Token: 0x06001233 RID: 4659 RVA: 0x00049694 File Offset: 0x00047894
        public float GetScaryMusicDistance()
        {
            return 55f;
        }

        // Token: 0x06001234 RID: 4660 RVA: 0x0004969B File Offset: 0x0004789B
        public float GetVeryScaryMusicDistance()
        {
            return 15f;
        }

        // Token: 0x06001235 RID: 4661 RVA: 0x000496A2 File Offset: 0x000478A2
        public float GetCrewmateInVentTime()
        {
            if (Settings[StringNames.MaxTimeInVent] is ModdedNumberOption moddedNumberOption)
            {
                return moddedNumberOption.IntValue;
            }
            return 3;
        }

        // Token: 0x06001236 RID: 4662 RVA: 0x000496AF File Offset: 0x000478AF
        public float GetCrewmateVentCooldown()
        {
            return 1f;
        }

        // Token: 0x06001237 RID: 4663 RVA: 0x000496B6 File Offset: 0x000478B6
        public float GetCommonTaskTimeValue()
        {
            return 10f - (float)GameData.Instance.PlayerCount * 1f / 2f;
        }

        // Token: 0x06001238 RID: 4664 RVA: 0x000496D5 File Offset: 0x000478D5
        public float GetShortTaskTimeValue()
        {
            return this.GetCommonTaskTimeValue();
        }

        // Token: 0x06001239 RID: 4665 RVA: 0x000496DD File Offset: 0x000478DD
        public float GetLongTaskTimeValue()
        {
            return 20f - (float)GameData.Instance.PlayerCount * 1f;
        }

        // Token: 0x0600123A RID: 4666 RVA: 0x000496F6 File Offset: 0x000478F6
        public bool GetSeekerFinalMap()
        {
            if (Settings[StringNames.SeekerFinalMap] is ModdedToggleOption moddedToggle)
            {
                return moddedToggle.BooleanValue;
            }
            return false;
        }

        // Token: 0x0600123B RID: 4667 RVA: 0x00049703 File Offset: 0x00047903
        public int ImpostorPlayerID()
        {
            return -1;
        }

        // Token: 0x0600123C RID: 4668 RVA: 0x00049710 File Offset: 0x00047910
        public bool HasImpostorPlayerID()
        {
            return false;
        }

        // Token: 0x0600123D RID: 4669 RVA: 0x00049720 File Offset: 0x00047920
        public bool ValidateImpostorPlayerID(List<NetworkedPlayerInfo> players)
        {
            return this.HasImpostorPlayerID() && players.Find((NetworkedPlayerInfo p) => (int)p.PlayerId == this.ImpostorPlayerID()) != null;
        }

        // Token: 0x0600123E RID: 4670 RVA: 0x00049744 File Offset: 0x00047944
        public bool GetSeekerPings()
        {
            if (Settings[StringNames.SeekerPings] is ModdedToggleOption moddedToggle)
            {
                return moddedToggle.BooleanValue;
            }
            return false;
        }

        // Token: 0x0600123F RID: 4671 RVA: 0x00049751 File Offset: 0x00047951
        public float GetMaxPingTime()
        {
            return this.GameOptions.MaxPingTime;
        }

        // Token: 0x06001240 RID: 4672 RVA: 0x0004975E File Offset: 0x0004795E
        public float GetShowPingTime()
        {
            return 2f;
        }

        // Token: 0x06001241 RID: 4673 RVA: 0x00049765 File Offset: 0x00047965
        public override bool GetShowCrewmateNames()
        {
            return Settings[];
        }
        public override global::TaskBarMode GetTaskBarMode()
        {
            return global::TaskBarMode.Normal;
        }
        public override int GetEmergencyCooldown()
        {
            return 0;
        }
        public override int GetNumEmergencyMeetings()
        {
            return 0;
        }
        public override bool GetVisualTasks()
        {
            return false;
        }
        public override bool GetGhostsDoTasks()
        {
            return false;
        }
        public override void FixedUpdate()
        {
            if (this.IsFinalCountdown)
            {
                this.AdjustFinalEscapeTimer(Time.fixedDeltaTime);
                return;
            }
            this.AdjustEscapeTimer(Time.fixedDeltaTime, false);
        }
        public override void CheckEndCriteria()
        {
            if (!GameData.Instance)
            {
                return;
            }
            ValueTuple<int, int, int> playerCounts = GetPlayerCounts();
            int item = playerCounts.Item1;
            int item2 = playerCounts.Item2;
            if (item2 <= 0 && !DestroyableSingleton<TutorialManager>.InstanceExists)
            {
                this.Manager.RpcEndGame(GameOverReason.ImpostorDisconnect, !DataManager.Player.Ads.HasPurchasedAdRemoval);
            }
            if (item > 0)
            {
                if (!DestroyableSingleton<TutorialManager>.InstanceExists && AllTimersExpired())
                {
                    this.Manager.RpcEndGame(GameOverReason.HideAndSeek_CrewmatesByTimer, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                }
                return;
            }
            if (!DestroyableSingleton<TutorialManager>.InstanceExists)
            {
                this.Manager.RpcEndGame(GameOverReason.HideAndSeek_ImpostorsByKills, !DataManager.Player.Ads.HasPurchasedAdRemoval);
                return;
            }
            DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills));
            this.Manager.ReviveEveryoneFreeplay();
        }
        public float GetRoundTimeElapsed()
        {
            return this.GetTotalRoundTime() - this.GetTotalTimeRemaining();
        }
        public float GetTotalRoundTime()
        {
            float escapeTime = this.hideAndSeekManager.LogicOptionsHnS.GetEscapeTime();
            float finalCountdownTime = this.hideAndSeekManager.LogicOptionsHnS.GetFinalCountdownTime();
            return escapeTime + finalCountdownTime;
        }
        public float GetTotalTimeRemaining()
        {
            return this.currentHideTime + this.currentFinalHideTime;
        }

        private void AdjustEscapeTimer(float timeDeduction, bool forceDirty)
        {
            float num = this.currentHideTime;
            this.currentHideTime -= timeDeduction;
            this.currentHideTime = Mathf.Max(this.currentHideTime, 0f);
            if (this.currentHideTime <= 10f && this.beepCoroutine == null)
            {
                this.beepCoroutine = Manager.StartCoroutine(this.BeepAlmostEverySecond().WrapToIl2Cpp());
            }
            if (num > 0f && this.currentHideTime <= 0f)
            {
                this.OnFinalCountdownTriggered();
            }
            this.timerBar.UpdateTimer(this.currentHideTime, this.totalHideTime);
        }
        private void OnFinalCountdownTriggered()
        {
            foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
            {
                if (!playerControl.Data.Role.IsImpostor && !playerControl.Data.IsDead)
                {
                    playerControl.ClearTasks();
                    PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0).Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.HideActionButton, Array.Empty<object>());
                }
            }
            if (!PlayerControl.LocalPlayer.Data.IsDead && Minigame.Instance != null)
            {
                Minigame instance = Minigame.Instance;
                if (instance != null)
                {
                    instance.ForceClose();
                }
            }
            this.timerBar.StartFinalHide();
            SoundManager.Instance.PlaySound(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX, false, 1f, null);
            DestroyableSingleton<HudManager>.Instance.SetAlertOverlay(true);
        }
        private void AdjustFinalEscapeTimer(float timeDeduction)
        {
            this.currentFinalHideTime -= timeDeduction;
            this.currentFinalHideTime = Mathf.Max(this.currentFinalHideTime, 0f);
            this.timerBar.UpdateTimer(this.currentFinalHideTime, this.totalFinalHideTime);
        }
        private System.Collections.IEnumerator BeepAlmostEverySecond()
        {
            while (!this.IsFinalCountdown)
            {
                float num = this.currentHideTime / 10f;
                float num2 = 1.5f - num / 2f;
                SoundManager.Instance.PlaySoundImmediate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideCountdownSFX, false, 1f, num2, null);
                yield return new WaitForSeconds(1f);
            }
            yield return Effects.Wait(this.currentFinalHideTime - 10f);
            while (this.currentFinalHideTime > 0f)
            {
                float num3 = this.currentFinalHideTime / 10f;
                float num4 = 1.5f - num3 / 2f;
                SoundManager.Instance.PlaySoundImmediate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideCountdownSFX, false, 1f, num4, null);
                yield return new WaitForSeconds(1f);
            }
            yield break;
        }

        private bool AllTimersExpired()
        {
            return this.currentHideTime <= 0f && this.currentFinalHideTime <= 0f;
        }
        protected ValueTuple<int, int, int> GetPlayerCounts()
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < GameData.Instance.PlayerCount; i++)
            {
                NetworkedPlayerInfo networkedPlayerInfo = GameData.Instance.AllPlayers[i];
                if (!(networkedPlayerInfo == null) && !networkedPlayerInfo.Disconnected && !(networkedPlayerInfo.Role == null))
                {
                    if (networkedPlayerInfo.Role.IsImpostor)
                    {
                        num3++;
                    }
                    if (!networkedPlayerInfo.IsDead)
                    {
                        if (networkedPlayerInfo.Role.IsImpostor)
                        {
                            num2++;
                        }
                        else
                        {
                            num++;
                        }
                    }
                    else
                    {
                        ImpostorGhostRole impostorGhostRole = networkedPlayerInfo.Role as ImpostorGhostRole;
                        if (impostorGhostRole != null && impostorGhostRole.WasManuallyPicked)
                        {
                            num2++;
                        }
                    }
                }
            }
            return new ValueTuple<int, int, int>(num, num2, num3);
        }








        public class HNSOptions : GameModeOptions
        {
            public override void Initialize(ModPlugin modPlugin) { }
        }
        internal class HNSGroup : SettingsGroup
        {
            public RulesCategory Category;
            public override StringNames GroupName => Category.CategoryName;
            public HNSGroup(RulesCategory rulesCategory, HideNSeekMode hideNSeekMode)
            {
                Category = rulesCategory;
                Options = new List<IModdedOption>();

                foreach (BaseGameSetting baseGameSetting in rulesCategory.AllGameSettings)
                {
                    if (baseGameSetting == null) continue;

                    BaseModdedOption moddedOption = null;

                    if (baseGameSetting.Is(out FloatGameSetting floatGameSetting))
                    {
                        moddedOption = new ModdedNumberOption(floatGameSetting.Title, floatGameSetting.Value, floatGameSetting.ValidRange.min, floatGameSetting.ValidRange.max, floatGameSetting.Increment, floatGameSetting.FormatString, floatGameSetting.ZeroIsInfinity, floatGameSetting.SuffixType);
                    }
                    else if (baseGameSetting.Is(out IntGameSetting intGameSetting))
                    {
                        moddedOption = new ModdedNumberOption(intGameSetting.Title, intGameSetting.Value, intGameSetting.ValidRange.min, intGameSetting.ValidRange.max, intGameSetting.Increment, intGameSetting.FormatString, intGameSetting.ZeroIsInfinity, intGameSetting.SuffixType);
                    }
                    else if (baseGameSetting.Is(out CheckboxGameSetting checkboxGameSetting))
                    {
                        moddedOption = new ModdedToggleOption(checkboxGameSetting.Title, false);
                    }

                    if (moddedOption != null)
                    {
                        moddedOption.StringOptionId = $"{moddedOption.Data.Title.ToString()}.{GetType().GetShortUniqueId()}";
                        moddedOption.OwnerPlugin = FungleApiPlugin.Plugin;
                        moddedOption.OptionId = OptionManager.__optionId;
                        OptionManager.__optionId++;

                        Options.Add(moddedOption);
                        hideNSeekMode.Settings.Add(moddedOption.Data.Title, moddedOption);
                    }
                }
            }
        }
    }
}
