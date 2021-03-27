using System;
using UnityEngine;

public class CelledItemSpace : IItemStoreSpace
{
    // LOW LEVEL VECTOR2INT SPACE FOR ITEMS
    private Vector2IntSpacing _space;

    // INT SIZE OF SPACE
    private Vector2Int _size;

    // DRAGGED ITEM RELATED. IF DRAGGED ITEM OVERLAPS ONLY ONE ITEM IN INVENTORY AND THEREFORE CAN EXCHANGE PLACES WITH IT
    private InventoryItem _toReplace;

    // TRACKS WHETHER IT IS TIME TO ASK FOR NEW HIGHLIGHT AREA
    private Vector2Int _lastCheckedCellCoord;
    
    public CelledItemSpace(Vector2IntSpaceData sizeData)
        => _space = new Vector2IntSpacing(_size = sizeData.SizeInt);

    public bool TryPlaceItemAuto(InventoryItem item, out Vector2 itemCenterNormalized)
    {
        itemCenterNormalized = Vector2.zero;
        if(_space.TryPlaceItemAuto(item))
        {
            itemCenterNormalized = GetItemCenterNormalized(item);
            return true;
        }
        return false;
    }

    public bool Empty()
        => _space.Empty();

    public bool CanPlaceItem(InventoryItem item, Vector2 normalizedLeftCornerPos)
    {
        _toReplace = null;
        _lastCheckedCellCoord = NormRectToInventoryCell(normalizedLeftCornerPos);

        if(_space.Exceeds(_lastCheckedCellCoord, item.SizeInt))
            return false;

        var overlaps = _space.GetOverlaps(_lastCheckedCellCoord, item.SizeInt);
        if(overlaps.Length == 1)
            _toReplace = (InventoryItem)overlaps[0];

        return overlaps.Length <= 1;
    }

    public bool CanPlaceItemAuto(InventoryItem item)
        => _space.TrySearchPlace(item.SizeInt, out Vector2Int pos);

    public bool PeekItem(Vector3 screenPos, out InventoryItem peeked)
    {
        peeked = null;
        if(_space.Exceeds(NormRectToInventoryCell(screenPos), new Vector2Int(1, 1)))
            return false;
        _space.PeekItem(NormRectToInventoryCell(screenPos), out IVector2IntItem peeked2);
        peeked = (InventoryItem)peeked2;
        return peeked != null;
    }

    public void PlaceItem(InventoryItem item, Vector2 leftCornerPosNormalized, out Vector2 posNormalized, out InventoryItem replaced)
    {  
        replaced = null;
        if(_toReplace != null)
        {
            _space.TryExtractItem(_toReplace.TopLeftCornerPosInt, out IVector2IntItem replaced2);
            replaced = (InventoryItem)replaced2;
        }
        _space.PlaceItemAtPos(item, NormRectToInventoryCell(leftCornerPosNormalized));
        posNormalized = GetItemCenterNormalized(item);
    }

    private Vector2 GetItemCenterNormalized(InventoryItem item)
    {
        float x = (float)item.TopLeftCornerPosInt.x + (float)item.SizeInt.x / 2;
        float y = (float)item.TopLeftCornerPosInt.y + (float)item.SizeInt.y / 2; 
        return new Vector2(x / (float)_size.x, y / (float)_size.y);
    }

    public void ExtractItem(InventoryItem toExtract, out InventoryItem extracted)
    {
        extracted = null;
        if(toExtract == null) return;

        _space.PeekItem(toExtract.TopLeftCornerPosInt, out IVector2IntItem peeked);
        if(peeked != null && (InventoryItem)peeked == toExtract)
        {
            _space.TryExtractItem(toExtract.TopLeftCornerPosInt, out IVector2IntItem extractedIntItem);
            extracted = (InventoryItem)extractedIntItem;
        }
    }

    public bool NeedHighlightRecalculation(Vector2 posNormalized)
    {
        var cell = NormRectToInventoryCell(posNormalized);
        if(_space.PeekItem(cell, out IVector2IntItem item))
        {
            _toReplace = (InventoryItem)item;
            cell = item.TopLeftCornerPosInt;
        }
        else 
            _toReplace = null;
        return cell != _lastCheckedCellCoord;
    }
    
    public bool NeedHighlightRecalculation(InventoryItem item, Vector2 leftCornerPosNormalized)
    {
        // IF OVERLAPPED ONLY ONE ITEM AND IT IS THE SAME AS PREVIOUS CALCULATION - RETURN FALSE
        var overlapped = _space.GetOverlaps(NormRectToInventoryCell(leftCornerPosNormalized), item.SizeInt);
        if(overlapped.Length == 1)
            return overlapped[0] != _toReplace;            

        // IF MORE THAN ONE OR NONE - RETURN IF THE CELL HAS CHANGED FROM THE PREVIOUS CALCULATION
        var cell = NormRectToInventoryCell(leftCornerPosNormalized);
        return cell != _lastCheckedCellCoord;
    }

    public Rect GetHighlightRectNormalized(InventoryItem item, Vector2 normalizedTopLeftCellCenter, out InventoryItem overlapped)
    {

        _lastCheckedCellCoord = NormRectToInventoryCell(normalizedTopLeftCellCenter);
        overlapped = _toReplace;

        if(_toReplace != null)
            _lastCheckedCellCoord = _toReplace.TopLeftCornerPosInt;
        
        return new Rect(GetCellPosNormalized(_lastCheckedCellCoord), _toReplace != null ? GetItemSizeNormalized(_toReplace) : GetItemSizeNormalized(item));
    }
    
    public Rect GetHighlightRectNormalized(Vector2 screenPosNormalized, out InventoryItem overlapped)
    {
        _lastCheckedCellCoord = NormRectToInventoryCell(screenPosNormalized);
        overlapped = null;
        
        if(_space.PeekItem(_lastCheckedCellCoord, out IVector2IntItem peeked))
        {
            _lastCheckedCellCoord = peeked.TopLeftCornerPosInt;
            overlapped = (InventoryItem)peeked;
            var normPos = GetCellPosNormalized(_lastCheckedCellCoord);
            var screenNormPos = new Vector2(normPos.x, 1 - normPos.y);
            return GetHighlightRectNormalized((InventoryItem)peeked, screenNormPos, out InventoryItem overlapped2);
        }

        Vector2 size = new Vector2(1f / (float)_size.x, 1 / (float)_size.y);
        return new Rect(GetCellPosNormalized(_lastCheckedCellCoord), size);
    }

    private Vector2 GetItemSizeNormalized(InventoryItem item)
        => (Vector2)item.SizeInt / (Vector2)_size;

    private Vector2 GetCellPosNormalized(Vector2Int cellPos)
        => (Vector2)cellPos / (Vector2)_size;

    private Vector2Int NormRectToInventoryCell(Vector2 normLeftCornerPos)
    {   
        float x = normLeftCornerPos.x / ( 1f / _size.x);
        float y = _size.y - normLeftCornerPos.y / ( 1f / _size.y);
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    public void RefreshHighlightInfo()
    {
        _lastCheckedCellCoord.Set(-1, -1);
        _toReplace = null;
    }
}