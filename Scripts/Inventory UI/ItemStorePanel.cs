using UnityEngine;
using UnityEngine.UI;

public abstract class ItemStorePanel : MonoBehaviour
{
    [SerializeField] 
    Image storePanel; 
    protected Image _storePanel => storePanel;

    [SerializeField] 
    Vector2IntSpaceData sizeData;
    protected Vector2IntSpaceData _sizeData => sizeData;
    
    protected Rect _panelRect;
    protected Canvas _parentCanvas;
    protected float _unitSize;


    public virtual void Init(Canvas parentCanvas)
    {
        _panelRect = new Rect(
            (Vector2)storePanel.transform.position - new Vector2(storePanel.rectTransform.sizeDelta.x / 2, storePanel.rectTransform.sizeDelta.y / 2), 
            storePanel.rectTransform.sizeDelta);
        _parentCanvas = parentCanvas;
        _unitSize = _panelRect.width / sizeData.SizeInt.x;
    }

    public bool ContainsPoint(Vector3 screenPos)
        => _panelRect.Contains(screenPos);

    public virtual bool ContainsItemCorners(InventoryItem item)
        => ContainsPoint(item.ScreenPos);

    public virtual bool NeedHighlightRecalculation(Vector3 cursorPos)
        => false;
    
    public virtual bool NeedHighlightRecalculation(InventoryItem item)
        => false;

    public abstract bool CanPlaceItem(InventoryItem item);

    public abstract void PlaceItem(InventoryItem item, out InventoryItem replaced);

    public abstract bool PeekItem(Vector3 screenPos, out InventoryItem peeked);

    public abstract void RemoveItem(InventoryItem toRemove);

    public abstract Rect GetHighlightRect(Vector3 screenPos);

    public abstract Rect GetHighlightRect(InventoryItem item);

    protected abstract void PlaceItemVisuals(InventoryItem item);

}
