
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    public abstract class ValueSource<T> : ScriptableObject, IValueSource<T>
    {
        public abstract T Value { get; protected set; }
    }
    
}
