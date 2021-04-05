using D2Inventory.Utils;
using UnityEngine;

namespace MNS.Utils.Values
{
    [CreateAssetMenu(menuName="Scriptable Objects/Values/Vector2 Handler Source")]
    public class Vector2HandlerSource : ValueSource<EnhancedEventHandler<Vector3>>
    {
        public override EnhancedEventHandler<Vector3> Value { get; protected set; } = new EnhancedEventHandler<Vector3>(); 
    }

}
