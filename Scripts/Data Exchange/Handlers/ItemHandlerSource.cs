using D2Inventory;
using MNS.Events;
using UnityEngine;

namespace MNS.Utils.Values
{

    [CreateAssetMenu(menuName="Scriptable Objects/Handlers/Item Handler Source")]
    public class ItemHandlerSource : ValueSource<EnhancedEventHandler<InventoryItem>>
    {
        public override EnhancedEventHandler<InventoryItem> Value { get; set; } = new EnhancedEventHandler<InventoryItem>();
    }

}
