using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities.Assets
{
    /// <summary>
    /// An asset that is created later during the loadAssets event
    /// </summary>
    public class LateAsset<T>
    {
        public Func<T> Create;
        /// <summary>
        /// The created asset instance
        /// </summary>
        public T Asset;
        public LateAsset(Func<T> create)
        {
            Create = create;
            FungleAPIPlugin.loadAssets += delegate
            {
                try
                {
                    Asset = Create();
                }
                catch (Exception ex)
                {
                    FungleAPIPlugin.Instance.Log.LogError("Failed to create late asset message:\n" + ex.Message);
                }
            };
        }
    }
}