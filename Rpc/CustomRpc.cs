using BepInEx.Unity.IL2CPP.Utils;
using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
using Hazel;
using InnerNet;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.CultureInfo;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Rpc
{
    public class CustomRpc
    {
        internal CustomRpc(ModPlugin plugin, RpcType type)
        {
            rpcType = type;
            Plugin = plugin;
            Id = Plugin.rpcId;
            Plugin.rpcId++;
        }
        internal RpcType rpcType;
        internal Action onSend = new Action(delegate
        {

        });
        internal Action onReceive;
        internal MessageReader Reader;
        internal MessageWriter Writer;
        internal ModPlugin Plugin;
        internal int Id;
        public void ResetMemory()
        {
            onSend = new Action(delegate
            {

            });
        }
        public void SendRpc(uint netId = 4294967295, SendOption sendOption = SendOption.Reliable, int targetClientId = -1)
        {
            try
            {
                uint id = 4294967295;
                if (netId == 4294967295)
                {
                    switch (rpcType)
                    {
                        case RpcType.Player:
                            id = PlayerControl.LocalPlayer.NetId;
                            break;
                        case RpcType.Ship:
                            id = ShipStatus.Instance.NetId;
                            break;
                        case RpcType.GameManager:
                            id = GameManager.Instance.NetId;
                            break;
                        case RpcType.Meeting:
                            id = MeetingHud.Instance.NetId;
                            break;
                        case RpcType.Lobby:
                            id = LobbyBehaviour.Instance.NetId;
                            break;
                    }
                }
                Writer = AmongUsClient.Instance.StartRpcImmediately(id, 70, sendOption, targetClientId);
                Writer.Write(Plugin.ModName);
                Writer.Write(Id);
                onSend();
                AmongUsClient.Instance.FinishRpcImmediately(Writer);
                ResetMemory();
                FungleAPIPlugin.Instance.Log.LogError("Sucefully sended RPC " + Id.ToString() + " from mod " + Plugin.ModName + ".");
            }
            catch
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to send RPC " + Id.ToString() + " from mod " + Plugin.ModName + ".");
            }
        }
        public void Write(string value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(int value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(byte value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(bool value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(float value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(PlayerControl value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(CustomDeadBody value)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(value);
            });
        }
        public void Write(Vector2 vector)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(vector.x);
                Writer.Write(vector.y);
            });
        }
        public void Write(Vector3 vector)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.Write(vector.x);
                Writer.Write(vector.y);
                Writer.Write(vector.z);
            });
        }
        public void Write(NetworkedPlayerInfo info)
        {
            Action ac = onSend;
            onSend = new Action(delegate
            {
                ac();
                Writer.WriteNetObject(info);
            });
        }
        public NetworkedPlayerInfo ReadNetObject()
        {
            return Reader.ReadNetObject<NetworkedPlayerInfo>();
        }
        public Vector2 ReadVector2()
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            return new Vector2(x, y);
        }
        public Vector2 ReadVector3()
        {
            float x = Reader.ReadSingle();
            float y = Reader.ReadSingle();
            float z = Reader.ReadSingle();
            return new Vector3(x, y, z);
        }
        public string ReadString()
        {
            return Reader.ReadString();
        }
        public int ReadInt()
        {
            return Reader.ReadInt32();
        }
        public byte ReadByte()
        {
            return Reader.ReadByte();
        }
        public bool ReadBool()
        {
            return Reader.ReadBoolean();
        }
        public float ReadFloat()
        {
            return Reader.ReadSingle();
        }
        public PlayerControl ReadPlayer()
        {
            return Utils.GetPlayerById(Reader.ReadByte());
        }
        public CustomDeadBody ReadBody()
        {
            return Utils.GetBodyById(Reader.ReadByte());
        }
    }
}
