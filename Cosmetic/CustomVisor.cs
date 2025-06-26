using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetic
{
    public class CustomVisor
    {
        public static List<CustomVisor> AllCustomVisors = new List<CustomVisor>();
        public CustomVisor(string visorId, string visorName, Sprite sprite)
        {
            VisorName = visorName;
            Sprite = sprite;
            VisorData = new VisorData();
            VisorData.ProductId = visorId;
            VisorData.Free = true;
            VisorData.NotInStore = true;
            AllCustomVisors.Add(this);
        }
        public string VisorName;
        public VisorData VisorData;
        public Sprite Sprite;
    }
}
