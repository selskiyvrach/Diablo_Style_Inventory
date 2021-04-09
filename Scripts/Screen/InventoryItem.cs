using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    
    public class InventoryItem : Vector2IntItem
    {
        public int ID { get; private set; }

        public InventoryItemData ItemData { get; private set; }

        public Vector2 DesiredScreenPos { get; set; }

        // TODO: set offset (sizeIint, unitSize) OR get corners in sontroller, substracting quarter of a square from the corners
        public Vector2 TopLeftCornerOffset { get; set; }

        public ContainerBase Container { get; set; }

        public InventoryItem(InventoryItemData data)
        {
            ItemData = data;
            SizeInt = ItemData.SizeInt;
            OneCellItem = SizeInt.magnitude < 2;
        }

        public void SetID(int value)
        {
            ID = value;
        }
    }
}