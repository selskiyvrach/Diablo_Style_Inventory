using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    
    public class InventoryItem : Vector2IntItem
    {
        public int ID { get; set; }

        public InventoryItemData ItemData { get; private set; }

        ///<summary>
        ///Represents last set position for item's icons. But doesn't return any actual value but that set from outside</summary>
        public Vector2 DesiredScreenPos { get; set; }

        public ContainerBase Container { get; set; }

        public int MainIconID { get; set; }

        ///<summary>
        ///Only used for two-handed items since they take both hands' slots when equipped</summary>
        public int SecondIconID { get; set; }

        public InventoryItem(InventoryItemData data)
        {
            ItemData = data;
            SizeInt = ItemData.SizeInt;
            OneCellItem = SizeInt.magnitude < 2;
        }
    }
}