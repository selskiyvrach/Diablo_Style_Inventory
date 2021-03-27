using UnityEngine;

[RequireComponent(typeof(ScreenRect))]
public abstract class ScreenSpaceItemContainer : MonoBehaviour
{
    [SerializeField] 
    Vector2IntSpaceData sizeData;
    public Vector2IntSpaceData SizeData => sizeData;

    [SerializeField] 
    ItemFitRule fitRule;
    protected ItemFitRule _fitRule => fitRule;

    [SerializeField] 
    ScreenRect screenRect;
    public ScreenRect ScreenRect => screenRect;

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

    public virtual bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
    {
        bool success = _storeSpace.PeekItem(screenRect.ScreenToNormalized(screenPos), out InventoryItem peeked2);
        peeked = peeked2;
        return success;
    }

    public Rect GetHighlightRect(InventoryItem item, out InventoryItem overlappedItem) 
    {
        var rect = screenRect.NormalizedRectToScreenRect(
            _storeSpace.GetHighlightRectNormalized(
                item, screenRect.ScreenToNormalized(item.GetCornerCenterInScreen(0, UnitSize)), out InventoryItem overlapped));
        overlappedItem = overlapped;
        return rect;
    }

    public Rect GetHighlightRect(Vector2 screenPos, out InventoryItem overlappedItem)
    {
        var pos = screenRect.ScreenToNormalized(screenPos);
        var rect = _storeSpace.GetHighlightRectNormalized(pos, out InventoryItem overlapped);
        overlappedItem = overlapped;
        return screenRect.NormalizedRectToScreenRect(rect);
    } 

    public bool NeedHighlightRecalculation(Vector2 screenPos)
        => _storeSpace.NeedHighlightRecalculation(screenRect.ScreenToNormalized(screenPos));

    public bool NeedHighlightRecalculation(InventoryItem item)
        => _storeSpace.NeedHighlightRecalculation(item, screenRect.ScreenToNormalized(item.GetCornerCenterInScreen(0, UnitSize)));

    public bool CanPlaceItem(InventoryItem item)
        => _storeSpace.CanPlaceItem(item, screenRect.ScreenToNormalized(item.GetCornerCenterInScreen(0, UnitSize)));

    public virtual void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {
        _storeSpace.PlaceItem(item, screenRect.ScreenToNormalized(item.GetCornerCenterInScreen(0, UnitSize)), out Vector2 itemCenterPosNormalized, out InventoryItem replaced2);
        replaced = replaced2;
        item.ScreenPos = screenRect.NormalizedRectPointToScreen(itemCenterPosNormalized);
    }

    public virtual void SetActive(bool value)
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

    public bool CanPlaceItemAuto(InventoryItem item)
        => _storeSpace.CanPlaceItemAuto(item);

    public virtual void ExtractItem(InventoryItem toExtract, out InventoryItem extracted)
    {
        _storeSpace.ExtractItem(toExtract, out InventoryItem extracted2);
        extracted = extracted2;
    }

    public void RefreshHighlightInfo()
        => _storeSpace.RefreshHighlightInfo();
}