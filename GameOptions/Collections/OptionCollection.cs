using FungleAPI.PluginLoading;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.GameOptions.Collections
{
    public abstract class OptionCollection
    {
        public bool Dirty;
        public string FilePath;
        public string CollectionId;
        public ModPlugin Plugin;
        public Dictionary<uint, IModdedOption> Options = new Dictionary<uint, IModdedOption>();

        public abstract void Initialize(Type type, ModPlugin modPlugin);
        public abstract void WriteLocalOptions();
        public abstract void ReadLocalOptions();
        public OptionCollection()
        {
            OptionManager.OptionCollections.Add(this);
        }
    }
}
