using FungleAPI.LoadMod;
using FungleAPI.MonoBehaviours;
using FungleAPI.Patches;
using FungleAPI.Roles;
using Hazel;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;

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
        internal List<object> WritedValues = new List<object>();
        internal RpcType rpcType;
        internal Action onReceive;
        internal MessageReader Reader;
        internal ModPlugin Plugin;
        internal int Id;
        public void ResetMemory()
        {
            WritedValues.Clear();
        }
        public void SendRpc()
        {
            try
            {
                uint id = 0;
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
                MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(id, 70, SendOption.Reliable, -1);
                messageWriter.Write(Plugin.ModName);
                foreach (var value in WritedValues)
                {
                    switch (value)
                    {
                        case string s:
                            messageWriter.Write(s);
                            break;
                        case int i:
                            messageWriter.Write(i);
                            break;
                        case byte bt:
                            messageWriter.Write(bt);
                            break;
                        case bool bl:
                            messageWriter.Write(bl);
                            break;
                        case float f:
                            messageWriter.Write(f);
                            break;
                    }
                }
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                WritedValues = new List<object>();
                FungleAPIPlugin.Instance.Log.LogError("Sucefully sended RPC " + Id.ToString() + " from mod " + Plugin.ModName + ".");
            }
            catch
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to send RPC " + Id.ToString() + " from mod " + Plugin.ModName + ".");
            }
        }
        public void Write(string value)
        {
            WritedValues.Add(value);
        }
        public void Write(int value)
        {
            WritedValues.Add(value);
        }
        public void Write(byte value)
        {
            WritedValues.Add(value);
        }
        public void Write(bool value)
        {
            WritedValues.Add(value);
        }
        public void Write(float value)
        {
            WritedValues.Add(value);
        }
        public void Write(PlayerControl value)
        {
            WritedValues.Add(value.PlayerId);
        }
        public void Write(CustomDeadBody value)
        {
            WritedValues.Add(value.Body.ParentId);
        }
        public void Write(Vector2 vector)
        {
            WritedValues.Add(vector.x);
            WritedValues.Add(vector.y);
        }
        public void Write(Vector3 vector)
        {
            WritedValues.Add(vector.x);
            WritedValues.Add(vector.y);
            WritedValues.Add(vector.z);
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
