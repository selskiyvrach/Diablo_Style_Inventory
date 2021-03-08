using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleItemSlotSpace : IItemStoreSpace
{
    private ItemFitRule _fitRule;
    private InventoryItem _content;

    private Vector2 _centerNormalized = new Vector2(0.5f, 0.5f);
    private Rect _highlightRectNormalized = new Rect(Vector2.zero, Vector2.one);

    public SingleItemSlotSpace(ItemFitRule fitRule)
        => _fitRule = fitRule;
    
    public bool CanPlaceItem(InventoryItem item, Vector2 normalizedRectPos)
    {
        if(_fitRule.CanFit(item.FitRule))
            return true;
        return false;
    }

    public bool Empty()
        => _content == null;

    public Rect GetHighlightRectNormalized(Vector2 normalizedScreenPos)
        => _highlightRectNormalized;

    public Rect GetHighlightRectNormalized(InventoryItem item, Vector2 normalizedRectPoint)
        => _highlightRectNormalized;

    public bool NeedHighlightRecalculation(Vector2 normalizedRectPoint)
        => false;

    public bool NeedHighlightRecalculation(InventoryItem item, Vector2 leftCornerPosNormalized)
        => false;

    public bool PeekItem(Vector3 normalizeRectPos, out InventoryItem peeked)
        => (peeked = _content) != null;

    public void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 itemSenterPosNormalized, out InventoryItem replaced)
    {
        itemSenterPosNormalized = _centerNormalized;
        replaced = _content;
        _content = item;
    }

    public void RemoveItem(InventoryItem toRemove)
        => _content = null;

    public bool TryPlaceItemAuto(InventoryItem item, out Vector2 itemSenterPosNormalized)
    {
        itemSenterPosNormalized = _centerNormalized;
        if(Empty())
            if(_fitRule.CanFit(item.FitRule))
            {
                PlaceItem(item, Vector2.zero, out Vector2 itemSenterPosNormalized2, out InventoryItem replaced);
                return true;
            }
        return false;
    }
}
