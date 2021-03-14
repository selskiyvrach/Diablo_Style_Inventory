
using UnityEngine;

public interface IItemStoreSpace
{
    // canvas related
    Rect GetHighlightRectNormalized(Vector2 screenPos, out InventoryItem overlappedItem);
    Rect GetHighlightRectNormalized(InventoryItem item, Vector2 normalizedTopLeftCellCenter, out InventoryItem overlappedItem);

    // item storage related
    bool Empty();
    bool PeekItem(Vector3 normalizeRectPos, out InventoryItem peeked);
    bool CanPlaceItem(InventoryItem item, Vector2 normalizedRectPos);
    bool CanPlaceItemAuto(InventoryItem item);
    bool TryPlaceItemAuto(InventoryItem item, out Vector2 itemSenterPosNormalized);
    void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 posNormalized, out InventoryItem replaced);
    void RemoveItem(InventoryItem toRemove);

    // highlight related
    bool NeedHighlightRecalculation(Vector2 normalizedRectPoint);
    bool NeedHighlightRecalculation(InventoryItem item, Vector2 leftCornerPosNormalized);

}

