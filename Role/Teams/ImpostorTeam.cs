using FungleAPI.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Role.Teams
{
    public class ImpostorTeam : ModdedTeam
    {
        public override bool FriendlyFire => false;
        public override bool KnowMembers => true;
        public override Color TeamColor => Palette.ImpostorRed;
        public override StringNames TeamName => StringNames.Impostor;
        public override int MaxCount
        {
            get
            {
                try
                {
                    return GameOptionsManager.Instance.currentGameOptions.GetInt(AmongUs.GameOptions.Int32OptionNames.NumImpostors);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}
