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
        public Dictionary<string, IModdedOption> Options = new Dictionary<string, IModdedOption>();

        public abstract void Serialize(MessageWriter messageWriter, bool includeOption);
        public abstract void Deserialize(MessageReader messageReader, bool includeOption);
        public abstract void Initialize(Type type, ModPlugin modPlugin);
        public abstract void WriteLocalOptions();
        public abstract void ReadLocalOptions();
        public OptionCollection()
        {
            OptionManager.OptionCollections.Add(this);
        }
    }
}
