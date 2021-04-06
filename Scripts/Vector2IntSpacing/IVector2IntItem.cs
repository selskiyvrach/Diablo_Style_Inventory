using UnityEngine;

namespace D2Inventory
{

    public interface IVector2IntItem
    {

        Vector2Int SizeInt { get; }

        Vector2Int TopLeftCornerPosInt { get; set; }

        bool OneCellItem { get; }

    }
    
}

