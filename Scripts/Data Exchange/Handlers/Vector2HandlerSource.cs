using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{
    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Vector2 Handler Source")]
    public class Vector2HandlerSource : ValueSource<EnhancedEventHandler<Vector3>>
    {
        public override EnhancedEventHandler<Vector3> Value { get; set; } = new EnhancedEventHandler<Vector3>(); 
    }

}
