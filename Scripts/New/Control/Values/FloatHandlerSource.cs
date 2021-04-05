using System;
using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Values/Float Handler Source")]
    public class FloatHandlerSource : ValueSource<EnhancedEventHandler<float>>
    {
        public override EnhancedEventHandler<float> Value { get; protected set; } = new EnhancedEventHandler<float>();
    }

}
