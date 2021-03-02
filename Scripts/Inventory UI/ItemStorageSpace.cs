using UnityEngine;

public class ItemStorageSpace : ItemStorePanel
{
    // LOW LEVEL VECTOR2INT SPACE FOR ITEMS
    private Vector2IntSpacing _space;

    // DRAGGED ITEM RELATED. IF DRAGGED ITEM OVERLAPS ONLY ONE ITEM IN INVENTORY AND THEREFORE CAN EXCHANGE PLACES WITH IT
    private InventoryItem _toReplace;

    // CORNERS OF STORE PANEL IN SCREEN SPACE. 0 - leftBottom, 1 - leftTop, 2 - rightTop 3 - rightBottom
    private Vector3[] _corners = new Vector3[4]; 

    // TRACKS WHETHER IT IS TIME TO ASK FOR NEW HIGHLIGHT AREA
    private Vector2Int _lastCheckedCellCoord;
    
    public override void Init(Canvas parentCanvas)
    {
        base.Init(parentCanvas);
        _space = new Vector2IntSpacing(_sizeData.SizeInt);
        _storePanel.rectTransform.GetWorldCorners(_corners);
    }

    public override bool ContainsItemCorners(InventoryItem item)
        => ContainsPoint(item.GetCornerCenterInScreen(0, _unitSize)) && ContainsPoint(item.GetCornerCenterInScreen(1, _unitSize));

    public override bool CanPlaceItem(InventoryItem item)
    {
        _lastCheckedCellCoord = ScreenPosToInventoryCell(item.GetCornerCenterInScreen(0, _unitSize));
        if(_space.Exceeds(_lastCheckedCellCoord, item.SizeInt))
        {
            _toReplace = null;
            return false;
        }
        var overlaps = _space.GetOverlaps(_lastCheckedCellCoord, item.SizeInt);
        if(overlaps.Length == 1)
            _toReplace = (InventoryItem)overlaps[0];
        return overlaps.Length <= 1;
    }

    public override bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
    {
        peeked = null;
        if(_space.Exceeds(ScreenPosToInventoryCell(screenPos), new Vector2Int(1, 1)))
            return false;
        _space.PeekItem(ScreenPosToInventoryCell(screenPos), out IVector2IntItem peeked2);
        peeked = (InventoryItem)peeked2;
        return peeked != null;
    }

    public override void PlaceItem(InventoryItem item, out InventoryItem replaced)
    {  
        replaced = null;
        if(_toReplace != null)
        {
            _space.TryExtractItem(_toReplace.TopLeftCornerPosInt, out IVector2IntItem replaced2);
            replaced = (InventoryItem)replaced2;
        }
        _space.PlaceItemAtPos(item, ScreenPosToInventoryCell(item.GetCornerCenterInScreen(0, _unitSize)));
        PlaceItemVisuals(item);
    }
    // NOT ACTUALLY USED HERE   
    public override void RemoveItem(InventoryItem toRemove)
        => _space.TryExtractItem(toRemove.TopLeftCornerPosInt, out IVector2IntItem extracted);

    protected override void PlaceItemVisuals(InventoryItem item)
    {
        item.EnableInventoryViewOfItem(_parentCanvas.transform, _unitSize);
        item.ScreenPos = CellCenterToScreen(item.TopLeftCornerPosInt) 
            + new Vector2(item.ScreenSize.x, - item.ScreenSize.y) / 2 
            - new Vector2(_unitSize, - _unitSize) / 2;
    }

    public override bool NeedHighlightRecalculation(Vector3 cursorPos)
        => ScreenPosToInventoryCell(cursorPos) != _lastCheckedCellCoord;
    
    public override bool NeedHighlightRecalculation(InventoryItem item)
        => ScreenPosToInventoryCell(item.GetCornerCenterInScreen(0, _unitSize)) != _lastCheckedCellCoord;

    public override Rect GetHighlightRect(Vector3 screenPos)
    {
        _lastCheckedCellCoord = ScreenPosToInventoryCell(screenPos);
        _toReplace = null;
        if(_space.PeekItem(_lastCheckedCellCoord, out IVector2IntItem peeked))
            return GetHighlightRect((InventoryItem)peeked);
        return new Rect(CellCenterToScreen(_lastCheckedCellCoord) - new Vector2(_unitSize / 2, _unitSize / 2), new Vector2(_unitSize, _unitSize));
    }

    public override Rect GetHighlightRect(InventoryItem item)
    {
        _lastCheckedCellCoord = ScreenPosToInventoryCell(item.GetCornerCenterInScreen(0, _unitSize));
        _toReplace = null;
        var size = new Vector2(item.SizeInt.x * _unitSize, item.SizeInt.y * _unitSize);
        var pos = CellCenterToScreen(_lastCheckedCellCoord) - new Vector2(_unitSize / 2, size.y - _unitSize / 2);
        return new Rect(pos, size);
    }

    private Vector2 CellCenterToScreen(Vector2Int cellPos)
    {
        float x = (_unitSize / 2) + (cellPos.x * _unitSize);
        float y = (_unitSize / 2) + (cellPos.y * _unitSize);
        return new Vector2(x, - y) + (Vector2)_corners[1];
    }

    private Vector2Int ScreenPosToInventoryCell(Vector2 screenPos)
    {   
        // SCREEN POS FROM TOP LEFT CORNER
        Vector2 relativePos = screenPos - (Vector2)_corners[1]; 
        // PROTECTION FROM WHEN SMALL -NEGATIVE- COORDS ROUND UP TO 0
        if(relativePos.x < 0 || relativePos.y > 0)
            return new Vector2Int(-1, -1); 
        // GETTING CELL NUMBER
        Vector2 squarePos = relativePos / _unitSize; 
        // SINCE SCREEN Y IS TOP TO BOTTOM CELL POS SHOULD HAVE NEGATIVE Y
        Vector2Int cellPos = new Vector2Int((int)squarePos.x, - (int)squarePos.y); 
        return cellPos;
    }
}
