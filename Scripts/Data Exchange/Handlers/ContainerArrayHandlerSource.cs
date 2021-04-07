using D2Inventory;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Container Array Handler Source")]
    public class ContainerArrayHandlerSource : ValueSource<EnhancedEventHandler<ContainerBase[]>>
    {
        public override EnhancedEventHandler<ContainerBase[]> Value { get; set; } = new EnhancedEventHandler<ContainerBase[]>();
    }

}
