using UnityEngine;

[RequireComponent(typeof(PreservedRatioRectTransform))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] RectTransform storePanel;
    [SerializeField] Canvas inventoryCanvas;
    public Canvas InventoryCanvas => inventoryCanvas;
    [SerializeField] Vector2IntSpaceData sizeData;
    [SerializeField] [Range(0, 1)] float highlighterAlpha;
    private Vector2IntSpacing _space;
    private UIHighlighter _highlighter;
    private Vector3[] _corners = new Vector3[4]; // 0 - leftBottom, 1 - leftTop, 2 - rightTop 3 - rightBottom
    private UIItemDragger _dragger;

// TRACKERS

    public float UnitSize { get; private set; }
    // CURSOR-WISE
    private Vector2 _cursorPos;
    private Vector2Int _cellCoord;
    private Vector2Int _prevCellCoord = new Vector2Int(-1, -1);
    // HIGHLIGHTED
    private Vector2 _highlightAreaSize;
    private Vector2 _highlightAreaPos;
    private InventoryItem _highlightedInventoryItem;
    // DRAGGED ITEM RELATED
    private bool _canReplaceOverlappedIfPresent;
    private InventoryItem _toReplace;

// PUBLIC

    public void TryAddItemAtItsCurrPos(InventoryItem newItem, out bool notOverlapsInventory, out bool cannotReplaceOverlappedItems, out InventoryItem replaced)
    {
        replaced = _toReplace;
        notOverlapsInventory = !_highlighter.Active;
        cannotReplaceOverlappedItems = !_canReplaceOverlappedIfPresent;

        if(!notOverlapsInventory && !cannotReplaceOverlappedItems)
        {
            if(_toReplace != null)
                _space.TryExtractItem(_toReplace.TopLeftCornerPosInt, out IVector2IntItem extracted);
            _space.PlaceItemAtPos(newItem, ScreenPosToInventoryCell(newItem.GetCornerCenterInScreen(0, UnitSize)));
            AnchorInventoryItemOnScreen(newItem);
            RecalculateHighlighting();
        }
    }
    
    public bool TryAddItemAuto(InventoryItem newItem)
    {
        if(_space.TryPlaceItemAuto(newItem))
        {
            AnchorInventoryItemOnScreen(newItem);
            return true;
        }
        return false;
    } 

    public bool TryExtractItemAtCursorPos(out InventoryItem item)
    {
        item = null;
        if(_highlightedInventoryItem != null)
        {
            _space.TryExtractItem(_highlightedInventoryItem.TopLeftCornerPosInt, out IVector2IntItem extracted);
            item = _highlightedInventoryItem;
            RecalculateHighlighting();
        }
        return item != null;
    }

// PRIVATE

    private void Awake() {
        _space = new Vector2IntSpacing(sizeData.SizeInt);
        _highlighter = new UIHighlighter(inventoryCanvas, highlighterAlpha);
        _dragger = new UIItemDragger(this);
        storePanel.GetWorldCorners(_corners);
        UnitSize = storePanel.sizeDelta.x / sizeData.SizeInt.x;
    }

    private void Update()
    {
        _dragger.ExternalUpdate();
        CheckIfNeededToRecalculateHighlight();
    }

    private void AnchorInventoryItemOnScreen(InventoryItem newItem)
    {  
        newItem.EnableInventoryViewOfItem(inventoryCanvas, UnitSize);
        newItem.ScreenPos = CellCenterToScreen(newItem.TopLeftCornerPosInt) 
            + new Vector2(newItem.ScreenSize.x, - newItem.ScreenSize.y) / 2 
            - new Vector2(UnitSize, - UnitSize) / 2;
        RecalculateHighlighting();
    }
 
    private void CheckIfNeededToRecalculateHighlight()
    {
        // IS CURSOR/DRAGGED ITEM INSIDE INVENTORY AREA
        bool cursorOnInventory = false;
        
        if(_dragger.Empty)
            cursorOnInventory = PosOverlapInventory(_cursorPos = Input.mousePosition);
        else
            cursorOnInventory = 
                PosOverlapInventory(_dragger.DraggedItem.GetCornerCenterInScreen(0, UnitSize)) && 
                PosOverlapInventory(_dragger.DraggedItem.GetCornerCenterInScreen(1, UnitSize));
        // CHECK IF CELL POS OF CURSOR HAS CHANGED
        if(cursorOnInventory)
        {
            if(_dragger.Empty)
                _cellCoord = ScreenPosToInventoryCell(_cursorPos);
            else 
                _cellCoord = ScreenPosToInventoryCell(_dragger.DraggedItem.GetCornerCenterInScreen(0, UnitSize));

            if(_cellCoord != _prevCellCoord)
                RecalculateHighlighting();

            _prevCellCoord = _cellCoord;
        }
        // TURN OFF AND RESET
        else if(_highlighter.Active)
        {
            _highlighter.Hide();
            _highlightedInventoryItem = null;
            _prevCellCoord.Set(-1, -1);
        }
    }

    private void RecalculateHighlighting()
    {
        _canReplaceOverlappedIfPresent = true;
        _toReplace = null;
        // DRAGGED ITEM
        if(!_dragger.Empty)
        {
            _highlightedInventoryItem = null;
            
            _cellCoord = ScreenPosToInventoryCell(_dragger.DraggedItem.GetCornerCenterInScreen(0, UnitSize));
            _highlightAreaSize = _dragger.DraggedItem.ScreenSize;
            _highlightAreaPos = CellCenterToScreen(_cellCoord) + new Vector2(_highlightAreaSize.x - UnitSize, - _highlightAreaSize.y + UnitSize) / 2;
            var overlaps = _space.GetOverlaps(_cellCoord, _dragger.DraggedItem.SizeInt);
            if(overlaps.Length == 1)
                _toReplace = (InventoryItem)overlaps[0];
            _canReplaceOverlappedIfPresent = overlaps.Length <= 1;
        }
        // INVENTORY ITEM
        else if(PosOverlapInventory(_cursorPos) && _space.PeekItem(_cellCoord, out IVector2IntItem item))
        {
            if((InventoryItem)item != _highlightedInventoryItem)
            {
                _highlightedInventoryItem = (InventoryItem)item;
                _highlightAreaSize = _highlightedInventoryItem.ScreenSize;
                _highlightAreaPos = _highlightedInventoryItem.ScreenPos;
            }
        }
        // EMPTY CELL
        else 
        {
            _highlightedInventoryItem = null;

            _highlightAreaSize = new Vector2(UnitSize, UnitSize);
            _highlightAreaPos = CellCenterToScreen(_cellCoord);
            _highlighter.NewHighlight(_highlightAreaPos, _highlightAreaSize);
        }
        _highlighter.NewHighlight(_highlightAreaPos, _highlightAreaSize, !_canReplaceOverlappedIfPresent);
    }

    public Vector2Int ScreenPosToInventoryCell(Vector2 screenPos)
    {   
        Vector2 relativePos = screenPos - (Vector2)_corners[1]; // screenPos from top lef corner
        if(relativePos.x < 0 || relativePos.y > 0)
            return new Vector2Int(-1, -1); // protection from when negative small nubers round up to 0

        Vector2 squarePos = relativePos / UnitSize; // divided by square size to get square number
        Vector2Int cellPos = new Vector2Int((int)squarePos.x, - (int)squarePos.y); // converted to int with negative y since it counts cell top to bottom
        return cellPos;
    }

    public Vector2 CellCenterToScreen(Vector2Int cellPos)
    {
        float x = (UnitSize / 2) + (cellPos.x * UnitSize);
        float y = (UnitSize / 2) + (cellPos.y * UnitSize);
        return new Vector2(x, - y) + (Vector2)_corners[1];
    }

    private bool PosOverlapInventory(Vector3 screenPos) => 
        screenPos.x > _corners[0].x && 
        screenPos.x < _corners[2].x && 
        screenPos.y > _corners[0].y && 
        screenPos.y < _corners[2].y;
}