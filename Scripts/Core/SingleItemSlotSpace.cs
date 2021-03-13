using UnityEngine;

public class SingleItemSlotSpace : IItemStoreSpace
{
    protected ItemFitRule _fitRule;
    public InventoryItem Content { get; protected set; }

    private Vector2 _centerNormalized = new Vector2(0.5f, 0.5f);
    private Rect _highlightRectNormalized = new Rect(Vector2.zero, Vector2.one);

    public SingleItemSlotSpace(ItemFitRule fitRule)
        => _fitRule = fitRule;
    
    public virtual bool CanPlaceItem(InventoryItem item, Vector2 normalizedRectPos)
    {
        if(_fitRule.CanFit(item.FitRule))
            return true;
        return false;
    }

    public virtual bool CanPlaceItemAuto(InventoryItem item)
        => Empty() && _fitRule.CanFit(item.FitRule);

    public bool Empty()
        => Content == null;

    public Rect GetHighlightRectNormalized(Vector2 normalizedScreenPos, out InventoryItem overlapped)
    {
        overlapped = Content;
        return _highlightRectNormalized;
    } 

    public Rect GetHighlightRectNormalized(InventoryItem item, Vector2 normalizedRectPoint, out InventoryItem overlapped)
    {
        overlapped = Content;
        return _highlightRectNormalized;
    }

    public bool NeedHighlightRecalculation(Vector2 normalizedRectPoint)
        => false;

    public bool NeedHighlightRecalculation(InventoryItem item, Vector2 leftCornerPosNormalized)
        => false;

    public bool PeekItem(Vector3 normalizeRectPos, out InventoryItem peeked)
        => (peeked = Content) != null;

    public virtual void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 itemSenterPosNormalized, out InventoryItem replaced)
    {
        itemSenterPosNormalized = _centerNormalized;
        replaced = Content;
        Content = item;
    }

    public void RemoveItem(InventoryItem toRemove)
    {
        if(toRemove == Content) 
            Content = null;
    }

    public virtual bool TryPlaceItemAuto(InventoryItem item, out Vector2 itemSenterPosNormalized)
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
