using D2Inventory;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{
    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Projection Handler Source")]
    public class ProjectionHandlerSource : ValueSource<EnhancedEventHandler<Projection>>
    {
        public override EnhancedEventHandler<Projection> Value { get; set; } = new EnhancedEventHandler<Projection>(); 
    }

}
