using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Assets
{
    public class GifFile : ScriptableObject
    {
        public GifFile(IntPtr ptr) : base(ptr) { }
        public bool Loop;
        public float[] Delays;
        public Sprite[] Sprites;
    }
}
