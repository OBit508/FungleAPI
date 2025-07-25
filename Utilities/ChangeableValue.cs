using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Utilities
{
    public class ChangeableValue<T>
    {
        public T Value;
        public ChangeableValue(T value)
        {
            Value = value;
        }
    }
}
