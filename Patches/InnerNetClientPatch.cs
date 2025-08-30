using AmongUs.InnerNet.GameDataMessages;
using FungleAPI.Networking;
using HarmonyLib;
using Hazel;
using InnerNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Patches
{
    [HarmonyPatch(typeof(InnerNetClient._HandleGameDataInner_d__165), "MoveNext")]
    internal static class InnerNetClientPatch
    {
        public static Dictionary<InnerNetClient._HandleGameDataInner_d__165, System.Collections.IEnumerator> AllIEnumerators = new Dictionary<InnerNetClient._HandleGameDataInner_d__165, System.Collections.IEnumerator>();
        public static bool Prefix(InnerNetClient._HandleGameDataInner_d__165 __instance, ref bool __result)
        {
            if (!AllIEnumerators.ContainsKey(__instance))
            {
                AllIEnumerators.Add(__instance, HandleGameDataInner(__instance, __instance.reader, __instance.msgNum));
            }
            System.Collections.IEnumerator enumerator;
            if (AllIEnumerators.TryGetValue(__instance, out enumerator))
            {
                __result = enumerator.MoveNext();
            }
            return false;
        }
        private static System.Collections.IEnumerator HandleGameDataInner(InnerNetClient._HandleGameDataInner_d__165 enumerator, MessageReader reader, int msgNum)
        {
            int cnt = 0;
            reader.Position = 0;
            GameDataTypes tag = (GameDataTypes)reader.Tag;

            switch (tag)
            {
                case GameDataTypes.DataFlag:
                    try
                    {
                        while (true)
                        {
                            uint netId = reader.ReadPackedUInt32();
                            lock (enumerator.__4__this.allObjects)
                            {
                                if (enumerator.__4__this.allObjects.AllObjectsFast.TryGetValue(netId, out var obj))
                                {
                                    obj.Deserialize(reader, false);
                                }
                                else if (!enumerator.__4__this.DestroyedObjects.Contains(netId))
                                {
                                    Debug.LogWarning("Stored data for " + netId);

                                    cnt++;
                                    if (cnt > 10) yield break;

                                    reader.Position = 0;
                                    yield return Effects.Wait(0.1f);
                                    continue;
                                }
                            }
                            break;
                        }
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                case GameDataTypes.RpcFlag:
                    try
                    {
                        while (true)
                        {
                            uint netId;
                            try
                            {
                                netId = reader.ReadPackedUInt32();
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error in {msgNum} try {cnt}, Pos:{reader.Position}/{reader.Length}: {ex}");
                                throw;
                            }

                            byte rpc = reader.ReadByte();
                            lock (enumerator.__4__this.allObjects)
                            {
                                if (netId == uint.MaxValue && rpc == byte.MaxValue)
                                {
                                    CustomRpcManager.CustomRead(reader);
                                }
                                else
                                {
                                    if (enumerator.__4__this.allObjects.AllObjectsFast.TryGetValue(netId, out var obj))
                                    {
                                        obj.HandleRpc(rpc, reader);
                                    }
                                    else if (netId != 4294967295 && !enumerator.__4__this.DestroyedObjects.Contains(netId))
                                    {
                                        Debug.LogWarning($"Stored Msg {msgNum} RPC {(RpcCalls)rpc} for {netId}");

                                        cnt++;
                                        if (cnt > 10) yield break;

                                        reader.Position = 0;
                                        yield return Effects.Wait(0.1f);
                                        continue;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                case GameDataTypes.SpawnFlag:
                    enumerator.__4__this.StartCoroutine(enumerator.__4__this.CoHandleSpawn(reader));
                    yield break;

                case GameDataTypes.DespawnFlag:
                    try
                    {
                        uint netId = reader.ReadPackedUInt32();
                        enumerator.__4__this.DestroyedObjects.Add(netId);

                        var obj = enumerator.__4__this.FindObjectByNetId<InnerNetObject>(netId);
                        if (obj && !obj.AmOwner)
                        {
                            enumerator.__4__this.RemoveNetObject(obj);
                            GameObject.Destroy(obj.gameObject);
                        }
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                case GameDataTypes.SceneChangeFlag:
                    try
                    {
                        int clientId = reader.ReadPackedInt32();
                        ClientData client = enumerator.__4__this.FindClientById(clientId);
                        string sceneName = reader.ReadString();

                        if (client != null && !string.IsNullOrWhiteSpace(sceneName))
                        {
                            enumerator.__4__this.StartCoroutine(enumerator.__4__this.CoOnPlayerChangedScene(client, sceneName));
                        }
                        else
                        {
                            Debug.Log($"Couldn't find client {clientId} to change scene to {sceneName}");
                        }
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                case GameDataTypes.ReadyFlag:
                    try
                    {
                        int clientId = reader.ReadPackedInt32();
                        ClientData client = enumerator.__4__this.FindClientById(clientId);
                        if (client != null)
                        {
                            client.IsReady = true;
                        }
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                case GameDataTypes.XboxDeclareXuid:
                    try
                    {
                        ulong.Parse(reader.ReadString());
                    }
                    finally
                    {
                        reader.Recycle();
                    }
                    yield break;

                default:
                    Debug.Log($"Bad tag {reader.Tag} at {reader.Offset}+{reader.Position}={reader.Length}: " +
                              string.Join(" ", reader.Buffer.Take(128)));
                    reader.Recycle();
                    yield break;
            }
        }

    }
}
