using BepInEx.Unity.IL2CPP;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.PluginLoading
{
    public interface IFungleBasePlugin
    {
        string ModName { get; }
        string ModVersion { get; }
        string ModCredits => null;
        bool UseShipReference => false;
        void LoadAssets()
        {
        }
        void OnRegisterInFungleAPI()
        {
        }
    }
}
