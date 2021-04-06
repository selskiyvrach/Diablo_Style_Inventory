
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    public abstract class ValueSource<T> : ScriptableObject, IValueSource<T>
    {
        public virtual T Value { get; set; }
    }
    
}
