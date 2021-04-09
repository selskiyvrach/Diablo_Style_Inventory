using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D2Inventory
{
    
    public class Vector2IntSpacing 
    {
        private Vector2Int _size;
        private Vector2IntItem[,] _space;

    // TRACKERS

        private List<Vector2IntItem> _overlaps = new List<Vector2IntItem>();

    // CONSTRUCTOR

        public Vector2IntSpacing(Vector2Int size)
            =>  _space = new Vector2IntItem[(_size = size).x, _size.y];

    // PUBLIC

        public bool TryPlaceItemAtPos(Vector2IntItem newItem, Vector2Int leftCornerPos)
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

        public bool TryPlaceItemAuto(Vector2IntItem newItem)
        {
            if(TrySearchPlace(newItem.SizeInt, out Vector2Int pos))
            {
                PutItemInSpace(newItem, pos);
                return true;
            }
            return false;
        }

        public void PlaceItemAtPos(Vector2IntItem newItem, Vector2Int leftCornerPos)
            => PutItemInSpace(newItem, leftCornerPos);

        public bool TryExtractItem(Vector2Int itemCornerSquare, out Vector2IntItem extracted)
        {
            extracted = _space[itemCornerSquare.x, itemCornerSquare.y];
            if(extracted != null)
                FreeCells(itemCornerSquare, extracted.SizeInt);
            return extracted != null;
        }

        public bool PeekItem(Vector2Int pos, out Vector2IntItem item)
            => (item = _space[pos.x, pos.y]) != null;

        public Vector2IntItem[] GetOverlaps(Vector2Int topLeftCornerPos, Vector2Int size)
        {
            _overlaps.Clear();
            ApplyActionToAreaIn2DSpace(topLeftCornerPos, size, (int x, int y) => 
            { 
                if(_space[x, y] != null && !_overlaps.Contains(_space[x, y]) )
                    _overlaps.Add(_space[x, y] ); 
            } );
            return _overlaps.ToArray();
        }

        public bool Empty()
            => !ApplyPredicateToAreaIn2DSpace(new Vector2Int(0, 0), _size, (int x, int y) => CellOccupied(x, y));

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

        public bool TrySearchPlace(Vector2Int itemSize, out Vector2Int pos)
        {
            pos = new Vector2Int(-1, -1);
            for(int x = 0; x < _size.x - (itemSize.x - 1); x++)
                for(int y = 0; y < _size.y - (itemSize.y - 1); y++)
                    if(!AnyOccupied(new Vector2Int(x, y), itemSize))
                    {
                        pos.Set(x, y);
                        return true;
                    }
            return false;
        }
        
        internal bool CanPlaceItemsAuto(Vector2IntItem[] items)
        {
            Vector2IntItem[] temp = new Vector2IntItem[items.Length]; 
            for(int n = 0; n < temp.Length; n++)
                temp[n] = new Vector2IntItem(items[n].SizeInt, items[n].TopLeftCornerPosInt);

            for(int i = 0; i < temp.Length; i++)
                if(!TryPlaceItemAuto(temp[i]))
                {
                    for(int j = 0; j < i; j++)
                        TryExtractItem(temp[j].TopLeftCornerPosInt, out Vector2IntItem extracted);
                    return false;
                }
            foreach (var item in temp)
                TryExtractItem(item.TopLeftCornerPosInt, out Vector2IntItem extracted);
            return true;
        }

        public bool TryPlaceItemsAuto(Vector2IntItem[] items)
        {
            if(CanPlaceItemsAuto(items))
            {
                foreach (var item in items)
                    TryPlaceItemAuto(item);
                return true;
            }
            return false;
        }

    // PRIVATE  

        private void PutItemInSpace(Vector2IntItem newItem, Vector2Int leftCornerPos)
        {
            ApplyActionToAreaIn2DSpace(leftCornerPos, newItem.SizeInt, (int x, int y) => _space[x, y] = newItem );
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
}