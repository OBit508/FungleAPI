using FungleAPI.Attributes;
using FungleAPI.GameMode;
using FungleAPI.GameOptions.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Components
{
    /// <summary>
    /// The component used in the lobby game mode tab
    /// </summary>
    [RegisterTypeInIl2Cpp]
    public class GameModeChanger : MonoBehaviour
    {
        public int CurrentIndex;
        public CustomGameMode CurrentGameMode;
        public TextMeshPro Text;
        public PassiveButton RightButton;
        public PassiveButton LeftButton;
        public void Awake()
        {
            Text = GetComponentInChildren<TextMeshPro>();
            RightButton = GetComponentsInChildren<PassiveButton>()[0];
            LeftButton = GetComponentsInChildren<PassiveButton>()[1];
            CurrentGameMode = GameModeManager.DefaultGameMode;
            Text.text = CurrentGameMode.GameModeName.GetString();
            RightButton.SetNewAction(new Action(delegate
            {
                if ((CurrentIndex + 1) >= GameModeManager.GameModes.Count)
                {
                    CurrentIndex = 0;
                }
                else
                {
                    CurrentIndex++;
                }
                CurrentGameMode = GameModeManager.GameModes.Values.ElementAt(CurrentIndex);
                Text.text = CurrentGameMode.GameModeName.GetString();
                Rpc<RpcSyncCurrentGameMode>.Instance.Send(PlayerControl.LocalPlayer);
            }));
            LeftButton.SetNewAction(new Action(delegate
            {
                if ((CurrentIndex - 1) <= -1)
                {
                    CurrentIndex = ModPluginManager.AllPlugins.Count - 1;
                }
                else
                {
                    CurrentIndex--;
                }
                CurrentGameMode = GameModeManager.GameModes.Values.ElementAt(CurrentIndex);
                Text.text = CurrentGameMode.GameModeName.GetString();
                Rpc<RpcSyncCurrentGameMode>.Instance.Send(PlayerControl.LocalPlayer);
            }));
        }
    }
}
