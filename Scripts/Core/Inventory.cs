using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] ScreenRect background;
    [SerializeField] InventoryHighlighter highlighter;
    [SerializeField] InventoryItemDragger dragger;
    [SerializeField] ContainersManager containersManager;
    
    // SET ONCE IN AWAKE
    private ScreenSpaceItemContainer _mainStorage; 
    // SET EACH TIME SINCE CONTAINS SWITCHABLE SLOTS
    private ScreenSpaceItemContainer[] _activeEquipmentSlots => containersManager.GetActiveEquipmentSlots();
    // SET EACH TIME SINCE CONTAINS SWITCHABLE SLOTS
    private ScreenSpaceItemContainer[] _allActiveContainers => containersManager.GetAllActiveContainers();

    // TRACKERS
    private float _unitSize;
    private ScreenSpaceItemContainer _currContainer = null;
    private Vector2 _cursorPos;
    private bool _overlapsBackground;

    public bool IsOn { get; private set; } 

// PUBLIC

    public void SetInventoryActive(bool value)
    {
        background.SetActive(IsOn = value);
        ForeachPanel(_allActiveContainers, (ScreenSpaceItemContainer p) => p.SetActive(value));
        InventoryItem.SetInventoryItemsActive(value);
        highlighter.gameObject.SetActive(value);
        
        if(value)
            InventoryEventsManager.OnInventoryOpened.Invoke(this, EventArgs.Empty);
        else 
        {
            Drop();
            InventoryEventsManager.OnInventoryClosed.Invoke(this, EventArgs.Empty);
        }
    }

    public void AddItemAuto(InventoryItemData itemData)
    {
        InventoryItem item = InventoryItemFactory.GetInventoryItem(itemData, _unitSize);
        item.EnableInventoryViewOfItem();

        if(IsOn)
            PickUp(item);
        else
            TryPlaceToContainersAuto(item);
    }

    public void SetCursorPos(Vector2 screenPos)
        => _cursorPos = screenPos;

    public void PerformPrimaryInteraction()
    {
        if(!dragger.Empty)
            PutItemInContainerOrDrop();
        else 
            RetrieveItemFromContainer();
    }

// PRIVATE  

    // MONOBEHAVIOUR

    private void Awake()
        => InitializeStuff();

    private void Start() 
        => SetInventoryActive(IsOn);

    private void Update()
    {
        if(IsOn)
        {
            _overlapsBackground = background.ContainsPoint(_cursorPos);

            if(_overlapsBackground)
                ForeachPanel(_allActiveContainers, CheckIfPointerIsOverAndItsCurrPanel);
            else 
                if(_currContainer != null)
                {
                    _currContainer = null;
                    TurnOffHighlight();
                }
        }
    }

    // UTILITY

    private void InitializeStuff()
    {
        _mainStorage = containersManager.GetMainStorage();
        _unitSize = _mainStorage.UnitSize;
        InventoryItem.Init(inventoryCanvas);
        highlighter.Initialize(inventoryCanvas);
    }

    private InventoryItemEventArgs GetDraggedItemEventArgs()
        => new InventoryItemEventArgs(dragger.DraggedItem, _currContainer, _cursorPos); 

    private void ForeachPanel(ScreenSpaceItemContainer[] arr, Action<ScreenSpaceItemContainer> toDo)
    {
        foreach(var p in arr)
            toDo(p);
    }

    // COMPLEX MANIPULATIONS

    private void TryPlaceToContainersAuto(InventoryItem item)
    {
        bool placed = false;
        // TRY PLACE TO EQUIPMENT SLOTS IF EMPTY
        ForeachPanel(_activeEquipmentSlots, (ScreenSpaceItemContainer p) =>
        {
            if (!placed && p.Empty() && p.CanPlaceItemAuto(item))
            {
                PlaceItemToSlot(item, p, new InventoryItemEventArgs(item, p, _cursorPos));
                InventoryEventsManager.OnItemEquipped.Invoke(this, new InventoryItemEventArgs(item, p, _cursorPos)); 
                placed = true;
            }
        });
        // TRY PLACE TO MAIN STORAGE
        if (!placed)
        {
            placed = _mainStorage.TryPlaceItemAuto(item);
            if(placed)
                InventoryEventsManager.OnItemEquipped.Invoke(this, new InventoryItemEventArgs(item, _mainStorage, _cursorPos)); 
        }
        // DROP BACK INTO WORLD
        if (!placed)
        {
            PickUp(item);
            Drop();
        }
    }

    private void PutItemInContainerOrDrop()
    {
        if(_currContainer != null)
            if(_currContainer.CanPlaceItem(dragger.DraggedItem))
                PlaceItemToSlot(dragger.DraggedItem, _currContainer, GetDraggedItemEventArgs());
            else
                InvokeImpossibleEvent();
        else 
            if(!_overlapsBackground)
                Drop(); 
    }

    // ELEMENTARY ACTIONS

    private void RetrieveItemFromContainer()
    {
        if (_currContainer != null && _currContainer.PeekItem(_cursorPos, out InventoryItem item))
            UnequipItem(_currContainer, item);
    }
    
    private void UnequipItem(ScreenSpaceItemContainer container, InventoryItem replaced)
    {
        container.RemoveItem(replaced);
        InventoryEventsManager.OnItemUnequipped.Invoke(this, new InventoryItemEventArgs(replaced, container, _cursorPos));
        PickUp(replaced);
    }

    private void PlaceItemToSlot(InventoryItem item, ScreenSpaceItemContainer container, InventoryItemEventArgs args)
    {
        container.PlaceItem(item, out InventoryItem replaced);
        InventoryEventsManager.OnItemEquipped.Invoke(this, args); 
        dragger.RemoveMouseFollower();
        if(replaced != null)
            UnequipItem(container, replaced);
    }

    private void PickUp(InventoryItem item)
    {
        if(!dragger.Empty)
            Drop();
        dragger.PickUp(item, false);
        InventoryEventsManager.OnItemTakenByCursor.Invoke(this, GetDraggedItemEventArgs());
    }

    private void Drop()
    {
        if(dragger.Empty) return;
        InventoryEventsManager.OnItemDroppedIntoWorld.Invoke(this, GetDraggedItemEventArgs());
        dragger.Drop();
    }

    private void InvokeImpossibleEvent()
        => InventoryEventsManager.OnImpossibleToProceed.Invoke(this, GetDraggedItemEventArgs());

    // CURRENT CONTAINER DETECTION && HIGHLIGHTING

    private void TurnOffHighlight()
    {
        highlighter.HideHighlight();
        InventoryEventsManager.OnHighlightOff.Invoke(this, EventArgs.Empty);
    }

    private void NewHighlight(bool canPlace, InventoryItem overlappedItem, Rect rect)
    {
        highlighter.NewHighlight(rect.center, rect.size, canPlace);
        InventoryEventsManager.OnNewHighlight.Invoke(this, new InventoryItemEventArgs(overlappedItem, _currContainer, _cursorPos));
    }

    private void CheckIfPointerIsOverAndItsCurrPanel(ScreenSpaceItemContainer c) 
    {
        // IF CURR CURSOR POS IS INSIDE PANEL'S AREA
        if(dragger.Empty ? 
            c.ContainsPoint(_cursorPos) :
            c == _mainStorage ? 
            c.ContainsItemCorners(dragger.DraggedItem) : 
            c.ContainsPoint(_cursorPos))
        {
            // IF THE CURSOR IS OVER A NEW CONTAINER OR THE CURRENT ONE NEEDS RECALCULATION 
            if(c != _currContainer || (dragger.Empty ? 
                _currContainer.NeedHighlightRecalculation(_cursorPos) : 
                _currContainer.NeedHighlightRecalculation(dragger.DraggedItem)))
            {
                _currContainer = c;
                InventoryItem overlappedItem = null;
                // NOTE: IMPORTANT TO FIRST CHECK IF CAN BE PLACED SINCE POTENTIALLY REPLACED ITEM WILL BE CASHED AND USED FURTHER IN DETERMINIG THE RECT
                var canPlace = dragger.Empty ? false : !_currContainer.CanPlaceItem(dragger.DraggedItem);
                var rect = new Rect();

                if (dragger.Empty)
                {
                    rect = _currContainer.GetHighlightRect(_cursorPos, out InventoryItem overlapped);
                    overlappedItem = overlapped;
                }
                else
                {
                    rect = _currContainer.GetHighlightRect(dragger.DraggedItem, out InventoryItem overlapped);
                    overlappedItem = overlapped;
                }
                NewHighlight(canPlace, overlappedItem, rect);
            }
        }
        else 
            if(c == _currContainer)
            {
                _currContainer = null;
                TurnOffHighlight();
            }
    }

}
