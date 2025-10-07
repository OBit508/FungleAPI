using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Freeplay
{
    internal class Debugger : ModFolderConfig
    {
        [Item("Show Intro", null, null)]
        public static Action ShowIntro { get; } = new Action(delegate
        {
            HudManager hud = HudManager.Instance;
            hud.StartCoroutine(hud.CoFadeFullScreen(Color.clear, Color.black));
            hud.StartCoroutine(hud.CoShowIntro());
        });
    }
}
