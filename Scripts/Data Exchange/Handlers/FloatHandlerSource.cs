using System;
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Float Handler Source")]
    public class FloatHandlerSource : ValueSource<EnhancedEventHandler<float>>
    {
        public override EnhancedEventHandler<float> Value { get; set; } = new EnhancedEventHandler<float>();
    }

}
