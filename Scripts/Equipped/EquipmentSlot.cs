using UnityEngine;

public class EquipmentSlot : ItemStorePanel
{
    [SerializeField] EquipmentFitType fitType;
    public EquipmentFitType FitType => fitType;

    private InventoryItem _content;        

    public override bool Empty()
        => _content == null;

    public override bool CanPlaceItem(InventoryItem item)
        => item.FitType != null && item.FitType == fitType;

    public override bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
        => (peeked = ContainsPoint(screenPos) ? _content : null) != null;

    public override void RemoveItem(InventoryItem toRemove)
        => _content = (toRemove == _content) ? null : _content;

    public override void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {
        replaced = null;
        if(CanPlaceItem(item))
        {
            replaced = _content;
            _content = item;
            PlaceItemVisuals(item);
        }
    }
    
    public override Rect GetHighlightRect(Vector3 screenPos)
        => _panelRect;

    public override Rect GetHighlightRect(InventoryItem item)
        => _panelRect;

    protected override void PlaceItemVisuals(InventoryItem item)
        => item.ScreenPos = _panelRect.center;

}
