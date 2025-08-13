using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Assets
{
    public class SpriteAnimation : ScriptableObject
    {
        public string AnimName;
        public bool Loop;
        public Dictionary<Sprite, ChangeableValue<float>> Frames;
        public Action StartAnim;
        public Action EndAnim;
    }
}
