using UnityEngine;

namespace D2Inventory
{

    public class Vector2IntItem
    {
        ///<summary>Represents size of an item in cells so it can be projected onto celled inventory space</summary>
        public Vector2Int SizeInt { get; protected set; }
        
        ///<summary>Used as a "position" point in vector2Int space</summary> 
        public Vector2Int TopLeftCornerPosInt { get; set; }

        ///<summary>Tells if an item is one cell size so functions can take computation speed advantage using this info</summary>
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

