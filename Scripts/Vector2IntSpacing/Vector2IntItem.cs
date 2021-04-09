using UnityEngine;

namespace D2Inventory
{

    public class Vector2IntItem
    {

        public Vector2Int SizeInt { get; protected set; }
        
        public Vector2Int TopLeftCornerPosInt { get; set; }

        public bool OneCellItem { get; protected set; }

        public Vector2IntItem(){}
        public Vector2IntItem(Vector2Int size, Vector2Int cornerPos = new Vector2Int())
        {
            SizeInt = size;
            TopLeftCornerPosInt = cornerPos; 
            OneCellItem = SizeInt.magnitude < 2;
        }
    }
    
}

