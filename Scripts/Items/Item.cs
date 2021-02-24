using UnityEngine;

public abstract class Item : IVector2IntSizeAndPos
{
    public ItemData ItemData { get; private set; }

    public Vector2Int SizeInt { get; private set; }

    public Vector2Int TopLeftCornerPosInt { get; set; }

    public Item TheItem { get; private set; }

    public Item(ItemData itemData)
    {
        ItemData = itemData;
        SizeInt = itemData.SizeInt;
        TheItem = this;
    }
}
