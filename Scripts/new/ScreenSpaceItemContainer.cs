using UnityEngine;

[RequireComponent(typeof(ScreenRect))]
public abstract class ScreenSpaceItemContainer : MonoBehaviour
{
    [SerializeField] Vector2IntSpaceData sizeData;
    protected Vector2IntSpaceData _sizeData => sizeData;
    [SerializeField] ItemFitRule fitRule;
    protected ItemFitRule _fitRule => fitRule;
    [SerializeField] ScreenRect screenRect;

    protected IItemStoreSpace _storeSpace;

    public float UnitSize => screenRect.Size.x / sizeData.SizeInt.x;

    private void Awake() 
        => Init();

    public abstract void Init();

    public bool Empty()
        => _storeSpace.Empty();
    
    public bool ContainsPoint(Vector2 screenPos)
        => screenRect.ContainsPoint(screenPos);

    public bool ContainsItemCorners(InventoryItem item)
        => item.OneCellItem ? 
            screenRect.ContainsPoint(item.ScreenPos) : 
            (screenRect.ContainsPoint(item.GetCornerCenterInScreen(0, UnitSize)) && screenRect.ContainsPoint(item.GetCornerCenterInScreen(1, UnitSize)));

    public bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
    {
        bool success = _storeSpace.PeekItem(screenRect.ScreenToRectNormalized(screenPos), out InventoryItem peeked2);
        peeked = peeked2;
        return success;
    }

    public Rect GetHighlightRect(InventoryItem item) //////////////////////////////////// OK
        => screenRect.NormalizedRectToScreenRect(_storeSpace.GetHighlightRectNormalized(item, screenRect.ScreenToRectNormalized(item.GetCornerCenterInScreen(0, UnitSize))));

    public Rect GetHighlightRect(Vector2 screenPos)
    {
        var pos = screenRect.ScreenToRectNormalized(screenPos);
        var rect = _storeSpace.GetHighlightRectNormalized(pos);
        return screenRect.NormalizedRectToScreenRect(rect);
    } //////////////////////////////////// NOT OK

    public bool NeedHighlightRecalculation(Vector2 screenPos)
        => _storeSpace.NeedHighlightRecalculation(screenRect.ScreenToRectNormalized(screenPos));

    public bool NeedHighlightRecalculation(InventoryItem item)
        => _storeSpace.NeedHighlightRecalculation(item, screenRect.ScreenToRectNormalized(item.GetCornerCenterInScreen(0, UnitSize)));

    public bool CanPlaceItem(InventoryItem item)
        => _storeSpace.CanPlaceItem(item, screenRect.ScreenToRectNormalized(item.GetCornerCenterInScreen(0, UnitSize)));

    public void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {
        _storeSpace.PlaceItem(item, screenRect.ScreenToRectNormalized(item.GetCornerCenterInScreen(0, UnitSize)), out Vector2 itemCenterPosNormalized, out InventoryItem replaced2);
        replaced = replaced2;
        item.ScreenPos = screenRect.NormalizedRectPointToScreen(itemCenterPosNormalized);
    }

    public void SetActive(bool value)
        => screenRect.SetActive(value);

    public bool TryPlaceItemAuto(InventoryItem item)
    {
        if(_storeSpace.TryPlaceItemAuto(item, out Vector2 itemSenterPosNormalized))
        {
            item.ScreenPos = screenRect.NormalizedRectPointToScreen(itemSenterPosNormalized);
            return true;
        }
        return false;
    }

    public void RemoveItem(InventoryItem toRemove)
        => _storeSpace.RemoveItem(toRemove);
}