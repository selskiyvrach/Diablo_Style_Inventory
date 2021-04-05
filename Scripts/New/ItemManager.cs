using MNS.Utils.Values;
using UnityEngine;

namespace D2Inventory
{

    public class ItemManager : MonoBehaviour
    {
        [SerializeField] int maxCapacity;
        
        public ItemData ItemData;

        private void Awake()
        {

            ItemData = new ItemData();
            
            ItemData.Capacity = maxCapacity;

            ItemData.iD = new int[maxCapacity];

            ItemData.ScreenPos = new Vector2[maxCapacity];

            ItemData.TopLeftCornerContainerPos = new Vector2Int[maxCapacity];

            ItemData.VisibleOnScreen = new bool[maxCapacity];

            ItemData.ItemInfo = new InventoryItemData[maxCapacity];

            for(int i = 0; i < maxCapacity; i++)
                ItemData.iD[i] = -1;

        }

        public int CreateItem(InventoryItemData itemData)
        {
            // TODO: check convention on return codes
            if(itemData == null) 
                return -404; // (:

            var index = GetFirstEmptyEntryIndex();

            if(index == -1)
                return -1;

            ItemData.ItemInfo[index] = itemData;
            return index;
        }   

        public void DeleteItem(int iD)
            => ItemData.iD[iD] = -1;

        private int GetFirstEmptyEntryIndex()
        {
            for(int i = 0; i < ItemData.Capacity; i++)
            {
                if(ItemData.iD[i] == -1)
                    return ItemData.iD[i] = i;
            }
            return -1;
        }
    }
    
}
