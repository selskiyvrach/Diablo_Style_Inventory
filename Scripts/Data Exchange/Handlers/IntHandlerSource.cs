using System;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Int Handler Source")]
    public class IntHandlerSource : ValueSource<EnhancedEventHandler<int>>
    {
        public override EnhancedEventHandler<int> Value { get; set; } = new EnhancedEventHandler<int>();
    }

}
