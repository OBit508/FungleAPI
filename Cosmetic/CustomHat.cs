using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Cosmetic
{
    public class CustomHat
    {
        public static List<CustomHat> AllCultomHats = new List<CustomHat>();
        public CustomHat(string hatId, string hatName, Sprite frontSprite)
        {
            HatName = hatName;
            Sprite = frontSprite;
            hatData = new HatData();
            hatData.ProductId = hatId;
            hatData.Free = true;
            hatData.NotInStore = true;
            AllCultomHats.Add(this);
        }
        public string HatName;
        public HatData hatData;
        public Sprite Sprite;
    }
}
