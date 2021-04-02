using System;
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Values/Handler Source")]
    public class HandlerSource : ValueSource<EnhancedEventHandler<EventArgs>>
    {
        public override EnhancedEventHandler<EventArgs> Value { get; protected set; } = new EnhancedEventHandler<EventArgs>();
    }

}
