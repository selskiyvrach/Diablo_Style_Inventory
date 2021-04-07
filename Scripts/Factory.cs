using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    public static class Factory 
    {

        public static T GetGameObjectOfType<T>(string Name = null) where T : Component
            => new GameObject(Name ??= typeof(T).ToString()).AddComponent(typeof(T)) as T;

        public static InventoryItem GetItem(InventoryItemData inventoryItemData)
            => new InventoryItem(inventoryItemData);
        
    }
    
}
