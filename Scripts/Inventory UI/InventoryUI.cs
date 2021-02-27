using UnityEngine;

[RequireComponent(typeof(PreservedRatioRectTransform))]
public class InventoryUI : MonoBehaviour
{
    // GAME OBJECTS
    [SerializeField] RectTransform storePanel;
    [SerializeField] Canvas inventoryCanvas;
    public Canvas InventoryCanvas => inventoryCanvas;

    // SCRIPTABLE OBJECTS
    [SerializeField] Vector2IntSpaceData sizeData;
    [SerializeField] InventorySettings settings;

    // CREATED HERE
    
    // LOW LEVEL VECTOR2INT SPACE OF INVENTORY
    private Vector2IntSpacing _space;
    // CORNERS OF STORE PANEL IN SCREEN SPACE
    private Vector3[] _corners = new Vector3[4]; // 0 - leftBottom, 1 - leftTop, 2 - rightTop 3 - rightBottom
    // HIGHLIGHTER OF AFFECTED INVENTORY CELLS  
    private UIHighlighter _highlighter;
    // KEEPS PICKED ITEM'S SCREEN POS ALONG WITH CURSOR POS
    private UIItemDragger _dragger;

// TRACKERS

    // SIZE OF INVENTORY CELL IN SCREEN SPACE COORDS
    public float UnitSize { get; private set; }
    // CURSOR-WISE
    private Vector2 _cursorPos;
    private Vector2Int _cellCoord;
    private Vector2Int _prevCellCoord = new Vector2Int(-1, -1);
    // HIGHLIGHTED
    private Vector2 _highlightAreaSize;
    private Vector2 _highlightAreaPos;
    private InventoryItem _highlightedInventoryItem;
    // DRAGGED ITEM RELATED 1
    private bool _canReplaceOverlappedIfPresent;
    // DRAGGED ITEM RELATED 2. IF DRAGGED ITEM OVERLAPS ONLY ONE ITEM IN INVENTORY AND THEREFORE CAN EXCHANGE PLACES WITH IT
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
        item = _highlightedInventoryItem;
        if(item != null)
        {
            _space.TryExtractItem(_highlightedInventoryItem.TopLeftCornerPosInt, out IVector2IntItem extracted);
            RecalculateHighlighting();
            return true;
        }
        return false;
    }

// PRIVATE

    private void Awake() {
        _space = new Vector2IntSpacing(sizeData.SizeInt);
        _highlighter = new UIHighlighter(inventoryCanvas, settings);
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
            cursorOnInventory = PosOverlapsInventory(_cursorPos = Input.mousePosition);
        else
            cursorOnInventory = 
                PosOverlapsInventory(_dragger.DraggedItem.GetCornerCenterInScreen(0, UnitSize)) && 
                PosOverlapsInventory(_dragger.DraggedItem.GetCornerCenterInScreen(1, UnitSize));
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
        else if(PosOverlapsInventory(_cursorPos) && _space.PeekItem(_cellCoord, out IVector2IntItem item))
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

    private bool PosOverlapsInventory(Vector3 screenPos) => 
        screenPos.x > _corners[0].x && 
        screenPos.x < _corners[2].x && 
        screenPos.y > _corners[0].y && 
        screenPos.y < _corners[2].y;

}