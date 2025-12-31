using BepInEx.Unity.IL2CPP;
using FungleAPI.Utilities;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace FungleAPI.PluginLoading
{
    public interface IFungleBasePlugin
    {
        string ModName { get; }
        string ModVersion { get; }
        string ModCredits => null;
        void LoadAssets()
        {
        }
        void OnRegisterInFungleAPI()
        {
        }
        System.Collections.IEnumerator CoLoadOnMainScreen(TextMeshPro loadingText)
        {
            yield return null;
        }
    }
}
