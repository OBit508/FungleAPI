using Amongus.GameModes.HideAndSeek;
using AmongUs.Data;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using FungleAPI.Base.Rpc;
using FungleAPI.Components;
using FungleAPI.Extensions;
using FungleAPI.GameModes;
using FungleAPI.GameOptions;
using FungleAPI.GameOptions.Options;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Modifiers;
using FungleAPI.Networking;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Utilities;
using Hazel;
using PowerTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace FungleAPI.Api
{
    public class HideNSeekMode : BaseGameMode
    {
        public Dictionary<StringNames, IModdedOption> Settings = new Dictionary<StringNames, IModdedOption>();

        private float currentHideTime = 100;
        private float currentFinalHideTime = 100;
        private float totalFinalHideTime = 100;
        private float totalHideTime = 100;

        private HideAndSeekTimerBar timerBar;
        private Coroutine beepCoroutine;

        private DangerMeter dangerMeter;
        private List<PlayerControl> impostors;
        private float scaryMusicDistance;
        private float veryScaryMusicDistance;
        private float dangerLevel1;
        private float dangerLevel2;

        private float syncTimer;

        private readonly Dictionary<HideAndSeekMusicTrack, string> musicNames = new Dictionary<HideAndSeekMusicTrack, string>
        {
            { HideAndSeekMusicTrack.Normal,       "HnS_Music_Normal" },
            { HideAndSeekMusicTrack.Task,         "HnS_Music_Task" },
            { HideAndSeekMusicTrack.DangerLevel1, "HnS_Music_DangerLevel1" },
            { HideAndSeekMusicTrack.DangerLevel2, "HnS_Music_DangerLevel2" },
        };

        public float HideCountdown;

        private bool isDoingTask;
        private float normalVolume;
        private float taskVolume;
        private float dangerLevel1Volume;
        private float dangerLevel2Volume;

        private AudioSource normalSource;
        private AudioSource taskSource;
        private AudioSource dangerLevel1Source;
        private AudioSource dangerLevel2Source;

        private float musicLerpSpeed = 5f;
        private float lastMusicSyncTime;
        private bool firstMusicActivation;
        private float firstCrossfadeCountdown;

        private ObjectPoolBehavior pingPool;
        private Coroutine seekerPingCoroutine;

        private int deadPlayerCount;

        public override StringNames GameModeName => StringNames.GameTypeHideAndSeek;
        public override GameModeOptions ModeOptions { get; } = new HNSOptions();

        public bool IsFinalCountdown => currentHideTime <= 0f;
        public float CurrentFinalHideTime => currentFinalHideTime;
        public float TotalFinalHideTime => totalFinalHideTime;
        public override IEnumerator CoIntroBegin(IntroCutscene introCutscene)
        {
            Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Starting intro cutscene", null);
            SoundManager.Instance.PlaySound(introCutscene.IntroStinger, false, 1f, null);
            Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek", null);
            introCutscene.LogPlayerRoleData();
            introCutscene.HideAndSeekPanels.SetActive(true);
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                introCutscene.CrewmateRules.SetActive(false);
                introCutscene.ImpostorRules.SetActive(true);
            }
            else
            {
                introCutscene.CrewmateRules.SetActive(true);
                introCutscene.ImpostorRules.SetActive(false);
            }
            Il2CppSystem.Collections.Generic.List<PlayerControl> list2 = IntroCutscene.SelectTeamToShow(new Func<NetworkedPlayerInfo, bool>((NetworkedPlayerInfo pcd) => PlayerControl.LocalPlayer.Data.Role.IsImpostor != pcd.Role.IsImpostor));
            if (list2 == null || list2.Count < 1)
            {
                Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
            }
            PlayerControl impostor = PlayerControl.AllPlayerControls.Find((PlayerControl pc) => pc.Data.Role.IsImpostor);
            if (impostor == null)
            {
                Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: impostor is NULL", null);
            }
            GameManager.Instance.SetSpecialCosmetics(impostor);
            introCutscene.ImpostorName.gameObject.SetActive(true);
            introCutscene.ImpostorTitle.gameObject.SetActive(true);
            introCutscene.BackgroundBar.enabled = false;
            introCutscene.TeamTitle.gameObject.SetActive(false);
            if (impostor != null)
            {
                introCutscene.ImpostorName.text = impostor.Data.PlayerName;
            }
            else
            {
                introCutscene.ImpostorName.text = "???";
            }
            yield return new WaitForSecondsRealtime(0.1f);
            PoolablePlayer playerSlot = null;
            if (impostor != null)
            {
                playerSlot = introCutscene.CreatePlayer(1, 1, impostor.Data, false);
                playerSlot.SetBodyType(PlayerBodyTypes.Normal);
                playerSlot.SetFlipX(false);
                playerSlot.transform.localPosition = introCutscene.impostorPos;
                playerSlot.transform.localScale = Vector3.one * introCutscene.impostorScale;
            }
            yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
            yield return new WaitForSecondsRealtime(6f);
            if (playerSlot != null)
            {
                playerSlot.gameObject.SetActive(false);
            }
            introCutscene.HideAndSeekPanels.SetActive(false);
            introCutscene.CrewmateRules.SetActive(false);
            introCutscene.ImpostorRules.SetActive(false);
            StartMusicWithIntro();
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                float crewmateLeadTime = (float)GetCrewmateLeadTime();
                introCutscene.HideAndSeekTimerText.gameObject.SetActive(true);
                PoolablePlayer poolablePlayer;
                AnimationClip animationClip;
                if (AprilFoolsMode.ShouldHorseAround())
                {
                    poolablePlayer = introCutscene.HorseWrangleVisualSuit;
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                    animationClip = introCutscene.HnSSeekerSpawnHorseAnim;
                    introCutscene.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
                    introCutscene.HorseWrangleVisualPlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                }
                else if (AprilFoolsMode.ShouldLongAround())
                {
                    poolablePlayer = introCutscene.HideAndSeekPlayerVisual;
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.SetBodyType(PlayerBodyTypes.LongSeeker);
                    animationClip = introCutscene.HnSSeekerSpawnLongAnim;
                }
                else
                {
                    poolablePlayer = introCutscene.HideAndSeekPlayerVisual;
                    poolablePlayer.gameObject.SetActive(true);
                    poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                    animationClip = introCutscene.HnSSeekerSpawnAnim;
                }
                poolablePlayer.SetBodyCosmeticsVisible(false);
                poolablePlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
                SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
                poolablePlayer.gameObject.SetActive(true);
                poolablePlayer.ToggleName(false);
                component.Play(animationClip, 1f);
                while (crewmateLeadTime > 0f)
                {
                    introCutscene.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
                    crewmateLeadTime -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                ShipStatus.Instance.HideCountdown = (float)GetCrewmateLeadTime();
                if (AprilFoolsMode.ShouldHorseAround())
                {
                    if (impostor != null)
                    {
                        impostor.AnimateCustom(introCutscene.HnSSeekerSpawnHorseInGameAnim);
                    }
                }
                else if (AprilFoolsMode.ShouldLongAround())
                {
                    if (impostor != null)
                    {
                        impostor.AnimateCustom(introCutscene.HnSSeekerSpawnLongInGameAnim);
                    }
                }
                else if (impostor != null)
                {
                    impostor.AnimateCustom(introCutscene.HnSSeekerSpawnAnim);
                    impostor.cosmetics.SetBodyCosmeticsVisible(false);
                }
            }
            impostor = null;
            playerSlot = null;
            ShipStatus.Instance.StartSFX();
            introCutscene.gameObject.Destroy();
            HideCountdown = PlayerControl.LocalPlayer.Data.Role.IsImpostor ? 0 : 10;
        }
        public override PlayerBodyTypes GetBodyType(PlayerControl player)
        {
            if (player == null || player.Data == null || player.Data.Role == null)
            {
                if (AprilFoolsMode.ShouldHorseAround())
                {
                    return PlayerBodyTypes.Horse;
                }
                if (AprilFoolsMode.ShouldLongAround())
                {
                    return PlayerBodyTypes.Long;
                }
                return PlayerBodyTypes.Normal;
            }
            else if (AprilFoolsMode.ShouldHorseAround())
            {
                if (player.Data.Role.IsImpostor)
                {
                    return PlayerBodyTypes.Normal;
                }
                return PlayerBodyTypes.Horse;
            }
            else if (AprilFoolsMode.ShouldLongAround())
            {
                if (player.Data.Role.IsImpostor)
                {
                    return PlayerBodyTypes.LongSeeker;
                }
                return PlayerBodyTypes.Long;
            }
            else if (AprilFoolsMode.ShouldClassicMode())
            {
                if (player.Data.Role.IsImpostor)
                {
                    return PlayerBodyTypes.Seeker;
                }
                return PlayerBodyTypes.Classic;
            }
            else
            {
                if (player.Data.Role.IsImpostor)
                {
                    return PlayerBodyTypes.Seeker;
                }
                return PlayerBodyTypes.Normal;
            }
        }
        public override MapOptions GetMapOptions()
        {
            MapOptions mapOptions = new MapOptions
            {
                Mode = MapOptions.Modes.Normal
            };
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor && SeekerAdminMapEnabled(PlayerControl.LocalPlayer))
            {
                mapOptions.Mode = MapOptions.Modes.CountOverlay;
                mapOptions.AllowMovementWhileMapOpen = true;
                mapOptions.IncludeDeadBodies = false;
                mapOptions.ShowLivePlayerPosition = false;
            }
            return mapOptions;
        }
        public bool SeekerAdminMapEnabled(PlayerControl player)
        {
            int item = GetPlayerCounts().Item1;
            return !player.inVent && !(player.Data == null) && !(player.Data.Role == null) && ((!player.inVent && player.Data.Role.IsImpostor && IsFinalCountdown && GetSeekerFinalMap()) || (player.Data.Role.IsImpostor && item <= (GameData.Instance.PlayerCount - 1) / 3));
        }
        public void OnTaskComplete(float timeDeduction)
        {
            if (timerBar != null)
            {
                timerBar.TaskComplete();
            }
            AdjustEscapeTimer(timeDeduction, true);
        }
        public override DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole)
        {
            return gameManager.deadBodyPrefab[impostorRole.GetCreatedDeadBody() == DeadBodyType.Viper ? 1 : 0];
        }
        public override void SetTaskPanelText(HudManager hudManager)
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (hudManager == null || localPlayer == null || localPlayer.Data == null || localPlayer.Data.Role == null || hudManager.tasksString == null)
            {
                return;
            }

            NetworkedPlayerInfo data = localPlayer.Data;
            if (localPlayer.myTasks != null)
            {
                for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
                {
                    PlayerTask playerTask = PlayerControl.LocalPlayer.myTasks[i];
                    if (playerTask)
                    {
                        if (playerTask.TaskType == TaskTypes.FixComms && !(data.Role != null && data.Role.IsImpostor))
                        {
                            hudManager.tasksString.Clear();
                            playerTask.AppendTaskText(hudManager.tasksString);
                            break;
                        }
                        playerTask.AppendTaskText(hudManager.tasksString);
                    }
                }
                if (data.Role != null)
                {
                    data.Role.AppendTaskHint(hudManager.tasksString);
                }
                if (HideCountdown > 0)
                {
                    hudManager.tasksString.Append("\n\n" + ((int)HideCountdown).ToString());
                }
                hudManager.tasksString.TrimEnd();
            }
        }
        public override float CalculateLightRadius(NetworkedPlayerInfo player, bool airship)
        {
            ShipStatus ship = ShipStatus.Instance;
            float Base()
            {
                if (player == null || player.IsDead)
                {
                    return ship.MaxLightRadius;
                }
                if (player.Role.IsImpostor)
                {
                    float impLight = 0;
                    if (Settings[StringNames.GameImpostorLight] is ModdedNumberOption moddedNumberOption)
                    {
                        impLight = moddedNumberOption.FloatValue;
                    }
                    return ship.MaxLightRadius * impLight;
                }
                float t = 1f;
                ISystemType systemType;
                if (ship.Systems.TryGetValue(SystemTypes.Electrical, out systemType))
                {
                    t = systemType.SafeCast<SwitchSystem>().Value / 255f;
                }

                float crewLight = 0;
                if (Settings[StringNames.GameCrewLight] is ModdedNumberOption moddedNumberOption2)
                {
                    crewLight = moddedNumberOption2.FloatValue;
                }

                return Mathf.Lerp(ship.MinLightRadius, ship.MaxLightRadius, t) * crewLight;
            }
            if (airship)
            {
                AirshipStatus airshipStatus = ship.SafeCast<AirshipStatus>();

                float num = Base();
                if (player.Role.AffectedByLightAffectors)
                {
                    foreach (LightAffector lightAffector in airshipStatus.LightAffectors)
                    {
                        if (player.Object && player.Object.Collider.IsTouching(lightAffector.Hitbox))
                        {
                            num *= lightAffector.Multiplier;
                        }
                    }
                }
                return num;
            }
            return Base();
        }
        public override void AdjustLighting(PlayerControl playerControl)
        {
            if (playerControl == null || playerControl.Data == null) return;

            float flashlightSize = 0f;
            if (IsFlashlightEnabled(playerControl))
            {
                if (playerControl.Data.Role.IsImpostor)
                {
                    if (Settings[StringNames.ImpostorFlashlightSize] is ModdedNumberOption moddedNumberOption)
                    {
                        flashlightSize = moddedNumberOption.FloatValue;
                    }
                }
                else
                {
                    if (Settings[StringNames.CrewmateFlashlightSize] is ModdedNumberOption moddedNumberOption)
                    {
                        flashlightSize = moddedNumberOption.FloatValue;
                    }
                }
            }
            playerControl.SetFlashlightInputMethod();
            playerControl.lightSource.SetupLightingForGameplay(IsFlashlightEnabled(playerControl), flashlightSize, playerControl.TargetFlashlight.transform);
        }
        public override bool IsFlashlightEnabled(PlayerControl playerControl)
        {
            if (LobbyBehaviour.Instance != null)
            {
                return false;
            }
            if (playerControl.Data.IsDead)
            {
                return false;
            }
            return Settings[StringNames.UseFlashlight] is ModdedToggleOption moddedToggleOption && moddedToggleOption.BooleanValue;
        }
        public override void OnGameStart()
        {
            pingPool = GameObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.PingPool);
            totalHideTime = GetEscapeTime();
            currentHideTime = totalHideTime;
            totalFinalHideTime = GetFinalCountdownTime();
            currentFinalHideTime = totalFinalHideTime;

            if (AmongUsClient.Instance.AmHost)
            {
                Rpc<RpcSyncTime>.Instance.Send(PlayerControl.LocalPlayer);
            }

            if (timerBar != null)
            {
                GameObject.Destroy(timerBar);
            }

            HudManager.Instance.TaskPanel.transform.parent.GetChild(1).gameObject.SetActive(false);

            timerBar = GameObject.Instantiate<HideAndSeekTimerBar>(GameManagerCreator.Instance.HideAndSeekManagerPrefab.TimerBarPrefab, DestroyableSingleton<HudManager>.Instance.transform.parent);

            firstMusicActivation = true;

            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                HudManager.Instance.DangerMeter = DestroyableSingleton<HudManager>.Instance.transform.GetChild(4).GetChild(0).GetComponent<DangerMeter>();
                dangerMeter = HudManager.Instance.DangerMeter;
                dangerMeter.gameObject.SetActive(true);
            }

            impostors = new List<PlayerControl>();
            foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
            {
                NetworkedPlayerInfo data = playerControl.Data;
                if (data?.Role != null && data.Role.IsImpostor)
                {
                    impostors.Add(playerControl);
                }
            }

            scaryMusicDistance = GetScaryMusicDistance() * GetSpeedMod();
            veryScaryMusicDistance = GetVeryScaryMusicDistance() * GetSpeedMod();
            if (scaryMusicDistance < veryScaryMusicDistance)
            {
                (scaryMusicDistance, veryScaryMusicDistance) = (veryScaryMusicDistance, scaryMusicDistance);
            }

            InitMusic();
            ResetMusic();
        }
        public override void OnGameEnd()
        {
            if (timerBar != null)
            {
                GameObject.Destroy(timerBar.gameObject);
            }

            if (beepCoroutine != null)
            {
                Manager.StopCoroutine(beepCoroutine);
            }
            beepCoroutine = null;

            impostors = null;
            ResetMusic();
            DestroyPingCoroutine();
        }
        public void UpdateGameFlow()
        {
            if (IsFinalCountdown)
            {
                AdjustFinalEscapeTimer(Time.fixedDeltaTime);
                return;
            }
            AdjustEscapeTimer(Time.fixedDeltaTime, false);
        }
        public void UpdateMeter()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (impostors == null || localPlayer == null)
            {
                return;
            }
            if (impostors.Count <= 0)
            {
                return;
            }
            float num = float.MaxValue;
            foreach (PlayerControl playerControl in impostors)
            {
                if (!(playerControl == null))
                {
                    float sqrMagnitude = (playerControl.transform.position - localPlayer.transform.position).sqrMagnitude;
                    if (sqrMagnitude < scaryMusicDistance && num > sqrMagnitude)
                    {
                        num = sqrMagnitude;
                    }
                }
            }
            if (HideCountdown > 0f)
            {
                dangerLevel1 = 0f;
                dangerLevel2 = 0f;
            }
            else
            {
                if (firstMusicActivation)
                {
                    firstMusicActivation = false;
                    firstCrossfadeCountdown = 3f;
                    SetMusicCrossfadeSpeed(0.6f);
                }
                if (firstCrossfadeCountdown > 0f)
                {
                    firstCrossfadeCountdown -= Time.deltaTime;
                    if (firstCrossfadeCountdown <= 0f)
                    {
                        SetMusicCrossfadeSpeed(5f);
                    }
                }
                dangerLevel1 = Mathf.Clamp01((scaryMusicDistance - num) / (scaryMusicDistance - veryScaryMusicDistance));
                dangerLevel2 = Mathf.Clamp01((veryScaryMusicDistance - num) / veryScaryMusicDistance);
            }
            UpdateDangerMeter();
            UpdateDangerMusic();
            ApplyMusicVolumes(Time.fixedDeltaTime);
        }
        public void UpdatePings()
        {
            if (!Manager.GameHasStarted)
            {
                return;
            }
            if (!IsFinalCountdown)
            {
                return;
            }
            if (seekerPingCoroutine != null)
            {
                return;
            }
            if (!GetSeekerPings())
            {
                return;
            }
            seekerPingCoroutine = Manager.StartCoroutine(SeekerPing().WrapToIl2Cpp());
        }
        public override void FixedUpdate()
        {
            if (AmongUsClient.Instance.IsGameStarted)
            {
                HideCountdown -= Time.fixedDeltaTime;

                UpdateGameFlow();
                UpdateMeter();
                UpdatePings();

                if (AmongUsClient.Instance.AmHost)
                {
                    syncTimer -= Time.fixedDeltaTime;
                    if (syncTimer <= 0)
                    {
                        Rpc<RpcSyncTime>.Instance.Send(PlayerControl.LocalPlayer);
                        syncTimer = 5;
                    }
                }
            }
        }
        public override void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
        {
            if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                return;
            }

            deadPlayerCount++;
            GameObject.Instantiate<HideAndSeekDeathPopup>(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, DestroyableSingleton<HudManager>.Instance.transform.parent).Show(player, deadPlayerCount);

            if (AmongUsClient.Instance.AmHost && assignGhostRole)
            {
                DestroyableSingleton<RoleManager>.Instance.AssignRoleOnDeath(player, false);
            }
        }
        public override void OnPlayerDisconnect(PlayerControl pc)
        {
            if (pc.Data.Role.IsImpostor)
            {
                impostors?.Remove(pc);
            }
        }
        public override bool CanUse(IUsable usable, PlayerControl player)
        {
            return !usable.Is(out Vent _) || !player.Data.Role.IsImpostor;
        }
        public override float GetPlayerSpeedMod(PlayerControl pc)
        {
            if (pc?.Data?.Role == null)
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

            float speed = GetSpeedMod() + GetSpeedMod() * 0.25f;
            if (IsFinalCountdown && Settings[StringNames.SeekerFinalSpeed] is ModdedNumberOption seekerFinalSpeed)
            {
                speed *= seekerFinalSpeed.FloatValue;
            }
            return speed;
        }
        private void ApplyMusicVolumes(float deltaTime)
        {
            if (normalSource == null || taskSource == null || dangerLevel1Source == null || dangerLevel2Source == null)
            {
                return;
            }

            float step = deltaTime * musicLerpSpeed;

            normalSource.volume = Mathf.MoveTowards(normalSource.volume, normalVolume, step);
            taskSource.volume = Mathf.MoveTowards(taskSource.volume, taskVolume, step);
            dangerLevel1Source.volume = Mathf.MoveTowards(dangerLevel1Source.volume, dangerLevel1Volume, step);
            dangerLevel2Source.volume = Mathf.MoveTowards(dangerLevel2Source.volume, dangerLevel2Volume, step);
        }
        public void ResetMusic()
        {
            SetMusicValues(0f, 0f);
        }
        public void SetMusicCrossfadeSpeed(float lerpSpeed)
        {
            musicLerpSpeed = lerpSpeed;
        }
        public void SetTaskState(bool isDoingTask)
        {
            this.isDoingTask = isDoingTask;
        }
        public void SetMusicValues(float dangerLevel1, float dangerLevel2)
        {
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                return;
            }

            if (normalSource == null || taskSource == null || dangerLevel1Source == null || dangerLevel2Source == null)
            {
                return;
            }

            normalVolume = isDoingTask ? 0f : 1f;
            taskVolume = isDoingTask ? 1f : 0f;
            dangerLevel1Volume = 0f;
            dangerLevel2Volume = 0f;

            if (dangerLevel1 > 0f)
            {
                dangerLevel1Volume = dangerLevel1;
                if (isDoingTask)
                {
                    taskVolume = 1f - dangerLevel1;
                }
                else
                {
                    normalVolume = 1f - dangerLevel1;
                }
            }

            if (dangerLevel2 > 0f)
            {
                dangerLevel2Volume = dangerLevel2;
                dangerLevel1Volume = 1f - dangerLevel2;
            }
        }
        public void StartMusicWithIntro()
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
            {
                return;
            }

            HideAndSeekMusicCollection musicCollection = GameManagerCreator.Instance.HideAndSeekManagerPrefab.MusicCollection;
            bool isShortGame = GetEscapeTime() <= 180f;

            AudioClip audioClip = isShortGame ? musicCollection.ImpostorShortMusic : musicCollection.ImpostorLongMusic;

            if (AprilFoolsMode.ShouldHorseAround())
            {
                audioClip = musicCollection.ImpostorRanchMusic;
            }

            SoundManager.Instance.PlaySound(audioClip, true, 1f, SoundManager.Instance.MusicChannel);
        }
        private void InitMusic()
        {
            HideAndSeekMusicCollection musicCollection = GameManagerCreator.Instance.HideAndSeekManagerPrefab.MusicCollection;
            AudioMixerGroup musicChannel = SoundManager.Instance.MusicChannel;

            if (normalSource == null)
            {
                normalSource = SoundManager.Instance.GetNamedSfxSource(musicNames[HideAndSeekMusicTrack.Normal]);
            }
            normalSource.outputAudioMixerGroup = musicChannel;
            normalSource.clip = musicCollection.NormalMusic;
            normalSource.loop = true;

            if (taskSource == null)
            {
                taskSource = SoundManager.Instance.GetNamedSfxSource(musicNames[HideAndSeekMusicTrack.Task]);
            }
            taskSource.outputAudioMixerGroup = musicChannel;
            taskSource.volume = 0f;
            taskSource.clip = musicCollection.TaskMusic;
            taskSource.loop = true;

            if (dangerLevel1Source == null)
            {
                dangerLevel1Source = SoundManager.Instance.GetNamedSfxSource(musicNames[HideAndSeekMusicTrack.DangerLevel1]);
            }
            dangerLevel1Source.outputAudioMixerGroup = musicChannel;
            dangerLevel1Source.volume = 0f;
            dangerLevel1Source.clip = musicCollection.DangerLevel1Music;
            dangerLevel1Source.loop = true;

            if (dangerLevel2Source == null)
            {
                dangerLevel2Source = SoundManager.Instance.GetNamedSfxSource(musicNames[HideAndSeekMusicTrack.DangerLevel2]);
            }
            dangerLevel2Source.outputAudioMixerGroup = musicChannel;
            dangerLevel2Source.volume = 0f;
            dangerLevel2Source.clip = musicCollection.DangerLevel2Music;
            dangerLevel2Source.loop = true;

            normalSource.Play();
            taskSource.Play();
            dangerLevel1Source.Play();
            dangerLevel2Source.Play();

            SyncMusic();
        }
        private void SyncMusic()
        {
            taskSource.timeSamples = normalSource.timeSamples;
            dangerLevel1Source.timeSamples = normalSource.timeSamples;
            dangerLevel2Source.timeSamples = normalSource.timeSamples;
            lastMusicSyncTime = Time.unscaledTime;
        }
        private void UpdateDangerMusic()
        {
            PlayerControl localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != null && localPlayer.Data != null && localPlayer.Data.IsDead)
            {
                SetTaskState(false);
                ResetMusic();
                return;
            }
            SetMusicValues(dangerLevel1, dangerLevel2);
        }
        public override void OnMinigameOpen() => SetTaskState(true);
        public override void OnMinigameClose() => SetTaskState(false);
        private void UpdateDangerMeter()
        {
            if (dangerMeter == null)
            {
                return;
            }
            dangerMeter.SetDangerValue(dangerLevel1, dangerLevel2);
        }
        private void DestroyPingCoroutine()
        {
            if (seekerPingCoroutine != null)
            {
                Manager.StopCoroutine(seekerPingCoroutine);
                seekerPingCoroutine = null;
            }
        }
        private IEnumerator SeekerPing()
        {
            while (Manager.GameHasStarted)
            {
                for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
                {
                    PlayerControl player = PlayerControl.AllPlayerControls[i];
                    bool isVisibleCrewmate = player.Data.Role.TeamType == RoleTeamTypes.Crewmate && !player.Data.IsDead;
                    bool shouldSeeThisPing = PlayerControl.LocalPlayer.Data.RoleType == RoleTypes.Impostor || player == PlayerControl.LocalPlayer;

                    if (isVisibleCrewmate && shouldSeeThisPing)
                    {
                        PingBehaviour pingBehaviour = pingPool.Get<PingBehaviour>();
                        pingBehaviour.target = player.GetTruePosition();
                        pingBehaviour.AmSeeker = PlayerControl.LocalPlayer.Data.RoleType == RoleTypes.Impostor;
                        pingBehaviour.UpdatePosition();
                        pingBehaviour.gameObject.SetActive(true);
                        pingBehaviour.SetImageEnabled(true);
                    }
                }

                yield return new WaitForSeconds(GetShowPingTime());

                foreach (PoolableBehavior poolableBehavior in pingPool.activeChildren)
                {
                    ArrowBehaviour arrowBehaviour = poolableBehavior.SafeCast<ArrowBehaviour>();
                    arrowBehaviour.target = Vector3.zero;
                    
                    if (arrowBehaviour.image != null)
                    {
                        arrowBehaviour.image.enabled = false;
                    }

                    arrowBehaviour.gameObject.SetActive(false);
                }

                yield return new WaitForSeconds(GetMaxPingTime());
            }
        }
        private void AdjustEscapeTimer(float timeDeduction, bool forceDirty)
        {
            float previousHideTime = currentHideTime;
            currentHideTime -= timeDeduction;
            currentHideTime = Mathf.Max(currentHideTime, 0f);

            if (currentHideTime <= 10f && beepCoroutine == null)
            {
                beepCoroutine = Manager.StartCoroutine(BeepAlmostEverySecond().WrapToIl2Cpp());
            }

            if (previousHideTime > 0f && currentHideTime <= 0f)
            {
                OnFinalCountdownTriggered();
            }

            timerBar?.UpdateTimer(currentHideTime, totalHideTime);
        }
        private void AdjustFinalEscapeTimer(float timeDeduction)
        {
            currentFinalHideTime -= timeDeduction;
            currentFinalHideTime = Mathf.Max(currentFinalHideTime, 0f);
            timerBar?.UpdateTimer(currentFinalHideTime, totalFinalHideTime);
        }
        private void OnFinalCountdownTriggered()
        {
            foreach (PlayerControl playerControl in PlayerControl.AllPlayerControls)
            {
                if (!playerControl.Data.Role.IsImpostor && !playerControl.Data.IsDead)
                {
                    playerControl.ClearTasks();
                    PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0).Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.HideActionButton);
                }
            }

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                Minigame.Instance?.ForceClose();
            }

            timerBar.StartFinalHide();
            SoundManager.Instance.PlaySound(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideAlertSFX, false, 1f, null);
            DestroyableSingleton<HudManager>.Instance.SetAlertOverlay(true);
        }
        private IEnumerator BeepAlmostEverySecond()
        {
            while (!IsFinalCountdown)
            {
                float progress = currentHideTime / 10f;
                float pitch = 1.5f - progress / 2f;
                SoundManager.Instance.PlaySoundImmediate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideCountdownSFX, false, 1f, pitch, null);
                yield return new WaitForSeconds(1f);
            }

            yield return Effects.Wait(currentFinalHideTime - 10f);

            while (currentFinalHideTime > 0f)
            {
                float progress = currentFinalHideTime / 10f;
                float pitch = 1.5f - progress / 2f;
                SoundManager.Instance.PlaySoundImmediate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.FinalHideCountdownSFX, false, 1f, pitch, null);
                yield return new WaitForSeconds(1f);
            }
        }
        public override void CheckEndCriteria()
        {
            if (!GameData.Instance)
            {
                return;
            }

            (int crewmatesAlive, int impostorsAlive, int impostorsTotal) = GetPlayerCounts();

            if (impostorsTotal <= 0 && !DestroyableSingleton<TutorialManager>.InstanceExists)
            {
                Manager.RpcEndGame<ImpostorDisconnect>();
            }

            if (crewmatesAlive > 0)
            {
                if (!DestroyableSingleton<TutorialManager>.InstanceExists && AllTimersExpired())
                {
                    Manager.RpcEndGame<CrewmatesByTask>();
                }
                return;
            }

            if (!DestroyableSingleton<TutorialManager>.InstanceExists)
            {
                Manager.RpcEndGame<ImpostorsByKill>();
                return;
            }

            DestroyableSingleton<HudManager>.Instance.ShowPopUp(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GameOverImpostorKills));
            Manager.ReviveEveryoneFreeplay();
        }
        protected ValueTuple<int, int, int> GetPlayerCounts()
        {
            int crewmatesAlive = 0;
            int impostorsAlive = 0;
            int impostorsTotal = 0;

            for (int i = 0; i < GameData.Instance.PlayerCount; i++)
            {
                NetworkedPlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                if (playerInfo == null || playerInfo.Disconnected || playerInfo.Role == null) continue;

                if (playerInfo.Role.IsImpostor)
                {
                    impostorsTotal++;
                }

                if (!playerInfo.IsDead)
                {
                    if (playerInfo.Role.IsImpostor)
                    {
                        impostorsAlive++;
                    }
                    else
                    {
                        crewmatesAlive++;
                    }
                }
            }

            return new ValueTuple<int, int, int>(crewmatesAlive, impostorsAlive, impostorsTotal);
        }
        public override int RequiredPlayerToStart() => 2;
        public float GetRoundTimeElapsed() => GetTotalRoundTime() - GetTotalTimeRemaining();
        public float GetTotalRoundTime() => GetEscapeTime() + GetFinalCountdownTime();
        public float GetTotalTimeRemaining() => currentHideTime + currentFinalHideTime;
        private bool AllTimersExpired() => currentHideTime <= 0f && currentFinalHideTime <= 0f;
        public float GetSpeedMod() => Settings[StringNames.GamePlayerSpeed] is ModdedNumberOption speedOption ? speedOption.FloatValue : 1f;
        public override float GetKillDistance() => HideNSeekGameOptionsV10.KillDistances[Mathf.Clamp(0, 0, HideNSeekGameOptionsV10.KillDistances.Length - 1)];
        public override float GetEngineerCooldown() => GetCrewmateVentCooldown();
        public override float GetEngineerInVentTime() => GetCrewmateInVentTime();
        public int GetCrewmateLeadTime() => 10;
        public float GetEscapeTime() => Settings[StringNames.EscapeTime] is ModdedNumberOption option ? option.FloatValue : 200;
        public float GetFinalCountdownTime() => Settings[StringNames.FinalEscapeTime] is ModdedNumberOption option ? option.FloatValue : 50;
        public int GetCrewmateVentUses() => Settings[StringNames.MaxVentUses] is ModdedNumberOption option ? option.IntValue : 1;
        public float GetScaryMusicDistance() => 55f;
        public float GetVeryScaryMusicDistance() => 15f;
        public float GetCrewmateInVentTime() => Settings[StringNames.MaxTimeInVent] is ModdedNumberOption option ? option.IntValue : 3;
        public float GetCrewmateVentCooldown() => 1f;
        public float GetCommonTaskTimeValue() => 10f - GameData.Instance.PlayerCount * 1f / 2f;
        public float GetShortTaskTimeValue() => GetCommonTaskTimeValue();
        public float GetLongTaskTimeValue() => 20f - GameData.Instance.PlayerCount * 1f;
        public bool GetSeekerFinalMap() => Settings[StringNames.SeekerFinalMap] is ModdedToggleOption option && option.BooleanValue;
        public int ImpostorPlayerID() => -1;
        public bool HasImpostorPlayerID() => false;
        public bool ValidateImpostorPlayerID(List<NetworkedPlayerInfo> players) => HasImpostorPlayerID() && players.Find(p => (int)p.PlayerId == ImpostorPlayerID()) != null;
        public bool GetSeekerPings() => Settings[StringNames.SeekerPings] is ModdedToggleOption option && option.BooleanValue;
        public float GetMaxPingTime() => Settings[StringNames.MaxPingTime] is ModdedNumberOption option ? option.FloatValue : 6f;
        public float GetShowPingTime() => 2f;
        public override bool GetShowCrewmateNames() => Settings[StringNames.ShowCrewmateNames] is ModdedToggleOption option && option.BooleanValue;
        public override int GetEmergencyCooldown() => 0;
        public override int GetNumEmergencyMeetings() => 0;
        public override bool GetVisualTasks() => false;
        public override bool GetGhostsDoTasks() => false;
        public override float GetKillCooldown() => 1;
        public override void SelectRoles(RoleManager roleManager)
        {
            if (PlayerControl.AllPlayerControls.Count > 0)
            {
                List<PlayerControl> playerControls = PlayerControl.AllPlayerControls.ToSystemList();

                PlayerControl seeker = playerControls.Random();
                playerControls.Remove(seeker);
                seeker.RpcSetRole(RoleTypes.Impostor);

                foreach (PlayerControl playerControl in playerControls)
                {
                    playerControl.RpcSetRole(RoleTypes.Engineer);
                }
            }
        }

        public enum HideAndSeekMusicTrack
        {
            None,
            Normal,
            Task,
            DangerLevel1,
            DangerLevel2
        }
        internal class RpcSyncTime : SimpleRpc<PlayerControl>
        {
            public override void Write(MessageWriter messageWriter)
            {
                messageWriter.Write(GameMode<HideNSeekMode>.Instance.currentHideTime);
                messageWriter.Write(GameMode<HideNSeekMode>.Instance.currentFinalHideTime);
            }
            public override void Handle(PlayerControl innerNetObject, MessageReader messageReader)
            {
                if (!AntiCheatManager.CheckForCheater(innerNetObject)) return;

                GameMode<HideNSeekMode>.Instance.currentHideTime = messageReader.ReadSingle();
                GameMode<HideNSeekMode>.Instance.currentFinalHideTime = messageReader.ReadSingle();
            }
        }
        internal class HNSOptions : GameModeOptions
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
                        moddedOption.StringOptionId = $"{moddedOption.Data.Title}.{GetType().GetShortUniqueId()}";
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