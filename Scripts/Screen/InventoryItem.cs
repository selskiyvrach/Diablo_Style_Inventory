using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{
    
    public class InventoryItem : IVector2IntItem, IIconed
    {
        public InventoryItemData ItemData { get; private set; }

        // IVector2IntItem:
        public Vector2Int TopLeftCornerPosInt { get; set; }
        public bool OneCellItem {get; private set; }

        // IIconed & IVector2IntItem:
        public Vector2Int SizeInt { get; private set; }

        // IIconed:
        public Image Icon { get; set; }
        public Sprite Sprite { get; private set; }
        public bool Visible { get; set; }
        public float Scale { get; private set; }

        public InventoryItem(InventoryItemData data)
        {
            ItemData = data;
            SizeInt = ItemData.SizeInt;
            OneCellItem = ItemData.OneCellItem;
            Sprite = ItemData.Sprite;
            Scale = ItemData.ImageScale;
        }
    }
}