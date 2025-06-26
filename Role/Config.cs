using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FungleAPI.LoadMod;
using BepInEx.Configuration;

namespace FungleAPI.Roles
{
    public class Config
    {
        internal Config()
        {
        }
        public string ConfigName;
        public int ConfigId;
        public ICustomRole Role;
    }
}
