using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PreservedRatioRectTransform))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] RectTransform storePanel;
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Vector2IntSpaceData sizeData;
    [SerializeField] float highlighterAlpha;

    private Vector2IntSpacing _space;
    private UIHighlighter _highlighter;
    private Vector3[] _corners = new Vector3[4]; // 0 - leftBottom, 1 - leftTop, 2 - rightTop 3 - rightBottom

    // trackers
    float _unitSize;
    Vector2 _cursorPos;
    Vector2Int _cellCoord;
    Vector2Int _prevCellCoord;
    Vector2 _highlightAreaSize;
    Vector2 _highlightAreaPos;
    UIItem _highlighted;

    private void Awake() {
        _space = new Vector2IntSpacing(sizeData.SizeInt);
        _highlighter = new UIHighlighter(inventoryCanvas, highlighterAlpha);
        storePanel.GetWorldCorners(_corners);
        _unitSize = storePanel.sizeDelta.x / sizeData.SizeInt.x;
    }
    
    public void AddItem(Item newItem)
    {
        var uIItem = newItem.CreateOrGetUIItem(_unitSize, inventoryCanvas);
        if(_space.TryPlaceItemAuto(newItem))
        {
            uIItem.transform.position = CellCenterToScreen(newItem.TopLeftCornerPosInt) 
                + new Vector2(uIItem.GetComponent<RectTransform>().sizeDelta.x, - uIItem.GetComponent<RectTransform>().sizeDelta.y) / 2 
                - new Vector2(_unitSize, - _unitSize) / 2;
            RecalculateHighlighting();
        }
    } 
    // test
    public void ExtractItem()
    {
        if(Input.GetMouseButtonDown(0))
            if(_highlighted != null)
                if(_space.TryExtractItem(_highlighted.TheItem.TopLeftCornerPosInt, out IVector2IntSizeAndPos item))
                {
                    _highlighted.TheItem.HideUIItem();
                    _space.PrintSpacing();
                    RecalculateHighlighting();
                }
    }

    private void Update() {
        _cursorPos = Input.mousePosition;

        if(PosOverlapInventory(_cursorPos))
        {
            _cellCoord = ScreenPosToInventoryCell(_cursorPos);

            if(!_highlighter.Active)
                RecalculateHighlighting(); // for the first entrance

            if(_cellCoord != _prevCellCoord)
                RecalculateHighlighting();

            _prevCellCoord = _cellCoord;
        }
        else
        {
            _highlighter.StayHidden();
            _highlighted = null;
        }
        // test
        ExtractItem();
    }

    private void RecalculateHighlighting()
    {
        if(_space.PeekItem(_cellCoord, out IVector2IntSizeAndPos item))
        {
            var candidate = ((Item)item).UIItem;
            if(_highlighted == candidate) return;

            _highlighted = candidate;
            _highlightAreaSize = GetItemScreenArea(item);
            _highlightAreaPos = GetItemScreenAreaPos(item, _highlightAreaSize);
            Debug.Log(_highlighted.TheItem.ItemData.Name);
        }
        else
        {
            _highlightAreaSize.Set(_unitSize, _unitSize);
            _highlightAreaPos = CellCenterToScreen(_cellCoord);
            _highlighted = null;
        }
        _highlighter.Highlight(_highlightAreaPos, _highlightAreaSize);
    }

    public Vector2 GetItemScreenArea(IVector2IntSizeAndPos item)
        => new Vector2(item.SizeInt.x * _unitSize, item.SizeInt.y * _unitSize);

    private Vector2 GetItemScreenAreaPos(IVector2IntSizeAndPos item, Vector2 screenAreaSize)
    {
        var topLeftCellCenter = CellCenterToScreen(item.TopLeftCornerPosInt);
        var topLeftCornerPos = new Vector2(topLeftCellCenter.x - _unitSize / 2, topLeftCellCenter.y + _unitSize /2);
        var halfAreaSize = new Vector2(screenAreaSize.x / 2, - screenAreaSize.y / 2);
        return topLeftCornerPos + halfAreaSize;
    }

    public Vector2Int ScreenPosToInventoryCell(Vector2 screenPos)
    {   
        Vector2 relativePos = screenPos - (Vector2)_corners[1]; // screenPos from top lef corner
        Vector2 squarePos = relativePos / _unitSize; // divided by square size to get square number
        Vector2Int cellPos = new Vector2Int((int)squarePos.x, - (int)squarePos.y); // converted to int with negative y since it counts cell top to bottom
        return cellPos;
    }

    public Vector2 CellCenterToScreen(Vector2Int cellPos)
    {
        float x = (_unitSize / 2) + (cellPos.x * _unitSize);
        float y = (_unitSize / 2) + (cellPos.y * _unitSize);
        return new Vector2(x, - y) + (Vector2)_corners[1];
    }

    public bool PosOverlapInventory(Vector3 screenPos)
        => screenPos.x > _corners[0].x && screenPos.x < _corners[2].x && screenPos.y > _corners[0].y && screenPos.y < _corners[2].y;
}
