using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D2Inventory
{

    public static class InventoryItemExtensions 
    {

        ///<param name="cornerNumber"> 
            ///0 = center of one-cell item/top-left corner of multi-cell item, 1 = bottom-right corner</param>
            ///<summary>
            ///0 = center of one-cell item/top-left corner of multi-cell item, 1 = bottom-right corner</summary>
        public static Vector2 GetCornerCenterInScreen(this InventoryItem item, int cornerNumber, float unitSize)
        {   
            Vector2 result = new Vector2();
            Vector2 screenPos = item.Icon.transform.position;
            var sizeInt = item.ItemData.SizeInt;

            cornerNumber = (int)Mathf.Clamp01(cornerNumber);
            // CENTER OF ONE-CELL ITEM
            if(item.ItemData.OneCellItem) 
                result = screenPos; 
            // CENTER OF TOP-LEFT CORNER CELL
            if(cornerNumber == 0) 
                result.Set(NegativeX(), PositiveY()); 
            // CENTER OF BOTTOM-RIGHT CORNER CELL
            if(cornerNumber == 1) 
                result.Set(PositiveX(), NegativeY()); 

            return result;

            float PositiveX() => sizeInt.x == 1 ? screenPos.x : (   (float)sizeInt.x / 2 - 0.5f) * unitSize + screenPos.x;
            float NegativeX() => sizeInt.x == 1 ? screenPos.x : ( - (float)sizeInt.x / 2 + 0.5f) * unitSize + screenPos.x;
            float PositiveY() => sizeInt.y == 1 ? screenPos.y : (   (float)sizeInt.y / 2 - 0.5f) * unitSize + screenPos.y;
            float NegativeY() => sizeInt.y == 1 ? screenPos.y : ( - (float)sizeInt.y / 2 + 0.5f) * unitSize + screenPos.y;
        }
    }
    
}
