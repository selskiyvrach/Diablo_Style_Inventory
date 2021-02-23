using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntSpacing 
{
    private Vector2Int _size;
    private IVector2IntItem[,] _space;
    private Dictionary<Vector2Int, IVector2IntItem> _items;

    public Vector2IntSpacing(Vector2Int size)
    {
        _size = size;
        _space = new IVector2IntItem[_size.x, _size.y];
        _items = new Dictionary<Vector2Int, IVector2IntItem>();
    }

    public bool TryPlaceItemAtPos(IVector2IntItem newItem, Vector2Int leftCornerPos)
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

    public bool PeekItem(Vector2Int pos, out IVector2IntItem item)
    {
        item = _space[pos.x, pos.y];
        return item != null;
    }

    public bool TryPlaceItemAuto(IVector2IntItem newItem, out Vector2Int topLeftCornerPos)
    {
        topLeftCornerPos = Vector2Int.zero;
        if(TrySearchPlace(newItem.SizeInt, out Vector2Int pos))
        {
            PutItemInSpace(newItem, pos);
            topLeftCornerPos = pos;
            return true;
        }
        Debug.Log($"Failed to put item in inventory. No place found");
        return false;
    }

    private bool TrySearchPlace(Vector2Int itemSize, out Vector2Int pos)
    {
        pos = new Vector2Int();

        for(int x = 0; x < _size.x - (itemSize.x - 1); x++)
        {
            for(int y = 0; y < _size.y - (itemSize.y - 1); y++)
            {
                if(!CellOccupied(x, y))
                {
                    pos.Set(x, y);
                    if(!AnyOccupied(pos, itemSize))
                        return true;
                } 
            }
        }
        return false;
    }

#region Unnecessary Production-wise Stuff

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
    }

    private string GetStringVisualizationOfCell(int x, int y)
        => CellOccupied(x, y) ? "[X]   " : "[_]    ";
    
#endregion

    private void PutItemInSpace(IVector2IntItem newItem, Vector2Int leftCornerPos)
    {
        ApplyActionToAreaIn2DSpace(leftCornerPos, newItem.SizeInt, (int x, int y) => OccupyCell(x, y, newItem));
        _items.Add(leftCornerPos, newItem);
    }

    public bool CanFit(Vector2Int pos, Vector2Int size)
        => !Exceeds(pos, size) && !AnyOccupied(pos, size);

    public bool AnyOccupied(Vector2Int pos, Vector2Int size)
        => ApplyPredicateToAreaIn2DSpace(pos, size, CellOccupied);

    public bool ApplyPredicateToAreaIn2DSpace(Vector2Int pos, Vector2Int size, Func<int, int, bool> toApply)
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

    private Vector2Int Constrain(Vector2Int toConstrain)
        => new Vector2Int(Mathf.Clamp(toConstrain.x, 0, _size.x - 1), Mathf.Clamp(toConstrain.y, 0, _size.y - 1));

    private void OccupyCell(int x, int y, IVector2IntItem item)
        => _space[x, y] = item;

    public bool Exceeds(Vector2Int pos, Vector2Int size)
        => ExceedsX(pos.x, size.x) || ExceedsY(pos.y, size.y);

    private bool ExceedsX(int xPos, int xSize)
        => xPos + (xSize - 1) >= _space.GetLength(0);

    private bool ExceedsY(int yPos, int ySize)
        => yPos + (ySize - 1) >= _space.GetLength(1);

    public bool CellAvailible(int xPos, int yPos)
        => CellPresent(xPos, yPos) && !CellOccupied(xPos, yPos);

    public bool CellOccupied(Vector2Int cell)
        => _space[cell.x, cell.y] != null;
    
    public bool CellOccupied(int xPos, int yPos)
        => _space[xPos, yPos] != null;

    public bool CellPresent(int xPos, int yPos)
        => xPos < _size.x && yPos < _size.y;
}
