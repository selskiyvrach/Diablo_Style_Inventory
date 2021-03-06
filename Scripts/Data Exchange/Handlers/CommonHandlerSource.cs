using System;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Common Handler Source")]
    public class CommonHandlerSource : ValueSource<EnhancedEventHandler<EventArgs>>
    {
        public override EnhancedEventHandler<EventArgs> Value { get; set; } = new EnhancedEventHandler<EventArgs>();
    }

}
