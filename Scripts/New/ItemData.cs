using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace D2Inventory
{

    public struct ItemData 
    {
        public int Capacity;

        public Vector2 cursorPos;

        public int[] iD;

        public Vector2[] ScreenPos;

        public Vector2Int[] TopLeftCornerContainerPos;

        public bool[] VisibleOnScreen;

        public InventoryItemData[] ItemInfo;

        public ItemData(int capacity)
        {
            Capacity = capacity;

            cursorPos = Vector2.zero;

            iD = new int[capacity];

            ScreenPos = new Vector2[capacity];

            TopLeftCornerContainerPos = new Vector2Int[capacity];

            VisibleOnScreen = new bool[capacity];

            ItemInfo = new InventoryItemData[capacity];

            for(int i = 0; i < capacity; i++)
                iD[i] = -1;
                
            for(int i = 0; i < capacity; i++)
                ScreenPos[i] = new Vector2(-1, -1);

            for(int i = 0; i < capacity; i++)
                TopLeftCornerContainerPos[i] = new Vector2Int(-1, -1);
        }
       
    }
    
}
