using AmongUs.GameOptions;
using FungleAPI.Attributes;
using FungleAPI.Extensions;
using FungleAPI.GameOver;
using FungleAPI.GameOver.Ends;
using FungleAPI.Modifiers;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using FungleAPI.Role;
using FungleAPI.Role.Utilities;
using FungleAPI.Teams;
using FungleAPI.Utilities;
using GameCore;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameModes
{
    [FungleIgnore]
    public abstract class BaseGameMode
    {
        public abstract StringNames GameModeName { get; }
        public uint GameModeId { get; internal set; }
        public GameManager Manager => GameManager.Instance;
        public IGameOptions GameOptions => Manager.LogicOptions.currentGameOptions;
        public virtual GameModeOptions ModeOptions { get; }
        public virtual int RequiredPlayerToStart() => 0;
        public virtual System.Collections.IEnumerator CoIntroBegin(IntroCutscene introCutscene) { yield break; }
        public virtual PlayerBodyTypes GetBodyType(PlayerControl player) => PlayerBodyTypes.Normal;
        public virtual void FixedUpdate() { }
        public virtual void OnGameStart() { }
        public virtual void OnGameEnd() { }
        public virtual void OnMinigameOpen() { }
        public virtual void OnMinigameClose() { }
        public virtual void OnPlayerDisconnect(PlayerControl pc) { }
        public virtual float CalculateLightRadius(NetworkedPlayerInfo player, bool airship) => 0;
        public virtual void AdjustLighting(PlayerControl playerControl) { }
        public virtual bool IsFlashlightEnabled(PlayerControl playerControl) => false;
        public virtual bool CanReportBodies() => true;
        public virtual bool CanUse(IUsable usable, PlayerControl player) => true;
        public virtual void OnPlayerDeath(PlayerControl player, bool assignGhostRole) { }
        public virtual int GetVotingTime() => 0;
        public virtual int GetDiscussionTime() => 0;
        public virtual bool GetShowCrewmateNames() => true;
        public virtual float GetEngineerInVentTime() => 0;
        public virtual float GetEngineerCooldown() => 0;
        public virtual TaskBarMode GetTaskBarMode() => default;
        public virtual bool GetAnonymousVotes() => false;
        public virtual bool GetVisualTasks() => true;
        public virtual bool GetChatInGame() => false;
        public virtual int GetNumEmergencyMeetings() => 1;
        public virtual int GetEmergencyCooldown() => 0;
        public virtual bool GetConfirmImpostor() => true;
        public virtual float GetPlayerSpeedMod(PlayerControl pc) => 0;
        public virtual float GetKillDistance() => 0;
        public virtual float GetKillCooldown() => 0;
        public virtual bool GetGhostsDoTasks() => false;
        public virtual void SetTaskPanelText(HudManager hudManager) { }
        public virtual float CanUseVent(Vent vent, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
        {
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            IUsable usable = vent.SafeCast<IUsable>();
            couldUse = (pc.Role.CanUseVent() || @object.AnyModifierForceVent()) && CanUse(usable, @object) && pc.Role.CanUse(usable) && (!@object.MustCleanVent(vent.Id) || (@object.inVent && Vent.currentVent == vent)) && !pc.IsDead && (@object.CanMove || @object.inVent);
            ISystemType systemType;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Ventilation, out systemType))
            {
                VentilationSystem ventilationSystem = systemType.SafeCast<VentilationSystem>();
                if (ventilationSystem != null && ventilationSystem.IsVentCurrentlyBeingCleaned(vent.Id))
                {
                    couldUse = false;
                }
            }
            canUse = couldUse;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = vent.transform.position;
                num = Vector2.Distance(center, position);
                canUse &= num <= vent.UsableDistance && !PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false);
            }
            return num;
        }
        public virtual float CanUseMapConsole(MapConsole mapConsole, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
        {
            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            couldUse = pc.Object.CanMove;
            canUse = couldUse;
            if (canUse)
            {
                num = Vector2.Distance(@object.GetTruePosition(), mapConsole.transform.position);
                canUse &= num <= mapConsole.UsableDistance;
            }
            return num;
        }
        public virtual MapOptions GetMapOptions() => null;
        public virtual DeadBody GetDeadBody(GameManager gameManager, RoleBehaviour impostorRole) => null;
        public virtual void SelectRoles(RoleManager roleManager) { }
        public virtual void AssignTasks(ShipStatus shipStatus) 
        {
            shipStatus.numScans = 0;
            shipStatus.AssignTaskIndexes();

            IGameOptions options = GameOptionsManager.Instance.CurrentGameOptions;
            Il2CppSystem.Collections.Generic.List<NetworkedPlayerInfo> players = GameData.Instance.AllPlayers;
            Il2CppSystem.Collections.Generic.HashSet<TaskTypes> used = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();
            Il2CppSystem.Collections.Generic.List<byte> tasks = new Il2CppSystem.Collections.Generic.List<byte>(10);
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> common = shipStatus.CommonTasks.ToList().ToIl2CppList();
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> longTasks = shipStatus.LongTasks.ToList().ToIl2CppList();
            Il2CppSystem.Collections.Generic.List<NormalPlayerTask> shortTasks = shipStatus.ShortTasks.ToList().ToIl2CppList();

            common.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle();
            longTasks.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            shortTasks.SafeCast<Il2CppSystem.Collections.Generic.IList<NormalPlayerTask>>().Shuffle(0);
            common.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Common);
            longTasks.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Long);
            shortTasks.ToSystemList().ForEach(t => t.Length = NormalPlayerTask.TaskLength.Short);

            int commonCount = options.GetInt(Int32OptionNames.NumCommonTasks);
            int longCount = options.GetInt(Int32OptionNames.NumLongTasks);
            int shortCount = options.GetInt(Int32OptionNames.NumShortTasks);

            if (commonCount + longCount + shortCount == 0)
            {
                shortCount = 1;
            }

            int commonIdx = 0;
            int longIdx = 0;
            int shortIdx = 0;

            shipStatus.AddTasksFromList(ref commonIdx, commonCount, tasks, used, common);

            for (int i = 0; i < commonCount && common.Count > 0; i++)
            {
                int idx = common.ToArray().RandomIdx();
                tasks.Add((byte)common[idx].Index);
                common.RemoveAt(idx);
            }

            for (byte i = 0; i < players.Count; i++)
            {
                used.Clear();
                tasks.RemoveRange(commonCount, tasks.Count - commonCount);

                shipStatus.AddTasksFromList(ref longIdx, longCount, tasks, used, longTasks);
                shipStatus.AddTasksFromList(ref shortIdx, shortCount, tasks, used, shortTasks);

                NetworkedPlayerInfo player = players[i];
                if (player.Object && !player.Object.GetComponent<DummyBehaviour>().enabled)
                {
                    player.RpcSetTasks((byte[])tasks.ToArray());
                }
            }
        }
        public virtual void CheckEndCriteria() { }
    }
}
