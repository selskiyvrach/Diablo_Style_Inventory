using UnityEngine;

public abstract class Item : IVector2IntItem
{
    public ItemData ItemData { get; private set; }

    public Vector2Int SizeInt { get; private set; }

    public Vector2Int TopLeftCornerPos { get; private set; }

    public Item TheItem { get; private set; }

    public Item(ItemData itemData)
    {
        ItemData = itemData;
        SizeInt = itemData.SizeInt;
        TheItem = this;
    }
}
