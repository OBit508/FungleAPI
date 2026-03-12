using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.Utilities.Assets.Late
{
    /// <summary>
    /// An asset that is created when called
    /// </summary>
    public abstract class LateAsset<T>
    {
        internal T __asset;
        /// <summary>
        /// The created asset instance
        /// </summary>
        public T Asset 
        {
            get
            {
                if (!Helpers.GameIsRunning)
                {
                    return default;
                }
                if (__asset == null)
                {
                    __asset = LoadAsset();
                }
                return __asset;
            }
        }
        protected abstract T LoadAsset();
        public static implicit operator T(LateAsset<T> lateAsset)
        {
            return lateAsset.Asset;
        }
    }
}