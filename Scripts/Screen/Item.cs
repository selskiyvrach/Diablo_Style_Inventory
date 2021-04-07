using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{

    public class Item 
    {
        // TODO: create id generator
        public int iD { get; private set; }

        public InventoryItemData Data { get; private set; }

        public Vector2 ScreenPos { get; private set; }

        public Vector2Int TopLeftCornerPosInContainer { get; set; }

        public Item(InventoryItemData data, int id)
        {
            Data = data;
            iD = id;
        }

    }
    
}
