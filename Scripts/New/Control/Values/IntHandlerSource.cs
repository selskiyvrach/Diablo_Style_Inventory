using System;
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Values/Int Handler Source")]
    public class IntHandlerSource : ValueSource<EnhancedEventHandler<int>>
    {
        public override EnhancedEventHandler<int> Value { get; protected set; } = new EnhancedEventHandler<int>();
    }

}
