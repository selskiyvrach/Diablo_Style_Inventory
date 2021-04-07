using System;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Bool Handler Source")]
    public class BoolHandlerSource : ValueSource<EnhancedEventHandler<bool>>
    {
        public override EnhancedEventHandler<bool> Value { get; set; } = new EnhancedEventHandler<bool>();
    }

}
