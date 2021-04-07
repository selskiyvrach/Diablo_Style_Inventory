using System;
using UnityEngine;

namespace MNS.Utils.Values
{

    public abstract class ChainValueSource<T> : ValueSource<T>
    {
        public Func<T> Getter;

        public override T Value => Getter.Invoke();
        
    }

}
