using FungleAPI.Freeplay.Helpers;
using FungleAPI.Player;
using FungleAPI.PluginLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Freeplay
{
    public class VanillaFolder : ModFolderConfig
    {
        public bool DebugLoaded;
        public override void Initialize(ModPlugin modPlugin)
        {
            base.Initialize(modPlugin);
            if (!DebugLoaded)
            {
                SubFolders.Add(new Folder()
                {
                    FolderName = "Debug",
                    FolderColor = new Color32(50, 50, 50, byte.MaxValue),
                    Items = new List<FolderItem>()
                    {
                        new FolderItem()
                        {
                            Name = "Self-Kill",
                            Color = Color.white,
                            OnClick = delegate
                            {
                                PlayerControl.LocalPlayer?.CheckCustomMurder(PlayerControl.LocalPlayer);
                            }
                        },
                        new FolderItem()
                        {
                            Name = "<size=1.8>Self-Protect</size>",
                            Color = Color.white,
                            OnClick = delegate
                            {
                                if (PlayerControl.LocalPlayer.protectedByGuardianId == -1)
                                {
                                    PlayerControl.LocalPlayer.TurnOnProtection(true, PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId, (int)PlayerControl.LocalPlayer.PlayerId);
                                }
                            }
                        }
                    }
                });
                DebugLoaded = true;
            }
        }
    }
}
