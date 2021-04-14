using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    
    public class InventoryItem : Vector2IntItem
    {
        
        public InventoryItemData ItemData { get; private set; }

        ///<summary>
        ///Represents last set position for item's icons. But doesn't return any actual value but that set from outside</summary>
        public Vector2 DesiredScreenPos { get; set; }

        public ContainerBase Container { get; set; }

// TODO: separate two-handed logic somehow...
// Two-handed related 

        ///<summary>
        ///Only used for two-handed items since they take both hands' slots when equipped</summary>

        public ContainerBase SecondTakenContainer { get; set; }

        public int[] IconIDs { get; private set; } // [0] - default icoon [1] - for two-handed

        public void InitIcons(int[] iDs)
        {
            for(int i = 0; i < IconIDs.Length; i++)
                IconIDs[i] = iDs[i];
        }

        public InventoryItem(InventoryItemData data)
        {
            ItemData = data;
            SizeInt = ItemData.SizeInt;
            OneCellItem = SizeInt.magnitude < 2;
            IconIDs = new int[data.TwoHanded ? 2 : 1];
        }
    }
}