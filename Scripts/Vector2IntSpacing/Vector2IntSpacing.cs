using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntSpacing 
{
    private Vector2Int _size;
    private IVector2IntSizeAndPos[,] _space;

// TRACKERS

    private List<IVector2IntSizeAndPos> _overlaps = new List<IVector2IntSizeAndPos>();

// CONSTRUCTOR

    public Vector2IntSpacing(Vector2Int size)
        =>  _space = new IVector2IntSizeAndPos[(_size = size).x, _size.y];

// PUBLIC

    public bool TryPlaceItemAtPos(IVector2IntSizeAndPos newItem, Vector2Int leftCornerPos)
    {
        if(!Exceeds(leftCornerPos, newItem.SizeInt))
        {
            if(!AnyOccupied(leftCornerPos, newItem.SizeInt))
            {
                PutItemInSpace(newItem, leftCornerPos);
                return true;
            }
            Debug.Log($"Failed to put item at {leftCornerPos}. One or more required cells are occupied");
        }
        Debug.Log($"Failed to put item at {leftCornerPos}. Item exceeds inventory boundaries");
        return false;
    }

    public bool TryPlaceItemAuto(IVector2IntSizeAndPos newItem)
    {
        if(TrySearchPlace(newItem.SizeInt, out Vector2Int pos))
        {
            PutItemInSpace(newItem, pos);
            return true;
        }
        Debug.Log($"Failed to put item in inventory. No place found");
        return false;
    }

    public void PlaceItemAtPos(IVector2IntSizeAndPos newItem, Vector2Int leftCornerPos)
        => PutItemInSpace(newItem, leftCornerPos);

    public bool TryExtractItem(Vector2Int itemCornerSquare, out IVector2IntSizeAndPos extracted)
    {
        extracted = _space[itemCornerSquare.x, itemCornerSquare.y];
        if(extracted != null)
            FreeCells(itemCornerSquare, extracted.SizeInt);
        return extracted != null;
    }

    public bool PeekItem(Vector2Int pos, out IVector2IntSizeAndPos item)
        => (item = _space[pos.x, pos.y]) != null;

    public IVector2IntSizeAndPos[] GetOverlaps(Vector2Int topLeftCornerPos, Vector2Int size)
    {
        _overlaps.Clear();
        ApplyActionToAreaIn2DSpace(topLeftCornerPos, size, (int x, int y) => 
        { 
            if(_space[x, y] != null && !_overlaps.Contains(_space[x, y]) )
                _overlaps.Add(_space[x, y] ); 
        } );
        return _overlaps.ToArray();
    }

    public bool Exceeds(Vector2Int pos, Vector2Int size) => 
        pos.x < 0 || 
        pos.y < 0 || 
        pos.x + (size.x - 1) >= _size.x || 
        pos.y + (size.y - 1) >= _size.y;

    public bool AnyOccupied(Vector2Int pos, Vector2Int size)
        => ApplyPredicateToAreaIn2DSpace(pos, size, CellOccupied);

    public void PrintSpacing()
    {
        string rowContent = "";
        for(int y = 0; y < _size.y; y++)
        {
            for(int x = 0; x < _size.x; x++)                
                rowContent += GetStringVisualizationOfCell(x, y);
            Debug.Log(rowContent);
            rowContent = "";
        }
        string GetStringVisualizationOfCell(int x, int y) => CellOccupied(x, y) ? 
            "[X]   " : 
            "[_]    ";
    }

// PRIVATE  

    private bool TrySearchPlace(Vector2Int itemSize, out Vector2Int pos)
    {
        pos = new Vector2Int();
        for(int x = 0; x < _size.x - (itemSize.x - 1); x++)
            for(int y = 0; y < _size.y - (itemSize.y - 1); y++)
                if(!CellOccupied(x, y))
                {
                    pos.Set(x, y);
                    if(!AnyOccupied(pos, itemSize))
                        return true;
                } 
        return false;
    }

    private void PutItemInSpace(IVector2IntSizeAndPos newItem, Vector2Int leftCornerPos)
    {
        ApplyActionToAreaIn2DSpace(leftCornerPos, newItem.SizeInt, (int x, int y) => _space[x, y] = newItem);
        newItem.TopLeftCornerPosInt = leftCornerPos;
    }

    private void FreeCells(Vector2Int startPos, Vector2Int areaSize)
        => ApplyActionToAreaIn2DSpace(startPos, areaSize, (int x, int y) => _space[x, y] = null);

    private bool CanFit(Vector2Int pos, Vector2Int size)
        => !Exceeds(pos, size) && !AnyOccupied(pos, size);

    private bool ApplyPredicateToAreaIn2DSpace(Vector2Int pos, Vector2Int size, Func<int, int, bool> toApply)
    {
        int finalX = pos.x + size.x; int finalY = pos.y + size.y;
        for(int x = pos.x; x < finalX; x++)
            for(int y = pos.y; y < finalY; y++)
                if(toApply(x, y)) 
                    return true;
        return false;
    }

    private void ApplyActionToAreaIn2DSpace(Vector2Int pos, Vector2Int size, Action<int, int> toApply)
    {
        int finalX = pos.x + size.x; int finalY = pos.y + size.y;
        for(int x = pos.x; x < finalX; x++)
            for(int y = pos.y; y < finalY; y++)
                toApply(x, y);
    }

    private bool CellOccupied(int xPos, int yPos)
        => _space[xPos, yPos] != null;
}