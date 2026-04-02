using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Assets.Late
{
    /// <summary>
    /// An AudioClip that is created when called
    /// </summary>
    public class LateAudio : LateAsset<AudioClip>
    {
        private Assembly __assembly;
        private string __resource;
        private string __clipName;
        public LateAudio(string Resource, string ClipName = null, Assembly assembly = null)
        {
            __assembly = assembly;
            if (__assembly == null)
            {
                __assembly = Assembly.GetCallingAssembly();
            }
            __resource = Resource;
            __clipName = ClipName;
        }
        protected override AudioClip LoadAsset()
        {
            AudioClip audioClip = null;
            try
            {
                FungleAPIPlugin.Instance.Log.LogInfo($"Created {__resource}");
                audioClip = AssetLoader.LoadAudio(__assembly, __resource, __clipName ?? __resource);
            }
            catch (Exception ex)
            {
                FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
            }
            return audioClip;
        }
    }
}
