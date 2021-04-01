using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using D2Inventory.Control;
using D2Inventory.Utils;
using System;

namespace D2Inventory
{   
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] Canvas inventoryCanvas;
        [Header("Controls")]
        [SerializeField] ControlUnit openClose;
        [SerializeField] ControlUnit switchWeapons;
        [SerializeField] ControlUnit leftClick;
        [SerializeField] Vector2ControlUnit mousePos;

        private ContainerBase _mainStorage;
        private ContainerBase[] _allContainers; 

        private InventoryItemDragger _dragger;
        private ContainerBase _currContainer;
        private Projection _proj;

        private float? _unitSize;

        private Vector2 _lastCursorPos;

        public bool Opened { get; private set; }
        public bool WeaponsSwitchedToFirstOpt { get; private set; }

        public EnhancedEventHandler<ProjectionEventArgs> OnProjectionChanged = new EnhancedEventHandler<ProjectionEventArgs>();
        public EnhancedEventHandler<BoolEventArgs> OnInventoryOpened = new EnhancedEventHandler<BoolEventArgs>();
        public EnhancedEventHandler<BoolEventArgs> OnWeaponsSwitchedToFirstOption = new EnhancedEventHandler<BoolEventArgs>();
        public EnhancedEventHandler<InventoryItemEventArgs> OnItemPickedUp = new EnhancedEventHandler<InventoryItemEventArgs>();
        public EnhancedEventHandler<InventoryItemEventArgs> OnCursorMoved = new EnhancedEventHandler<InventoryItemEventArgs>();


        private void Awake()
        {
            _dragger = new InventoryItemDragger(inventoryCanvas);

            OnProjectionChanged.Invoke(this, ProjectionEventArgs.EmptyProjection);
            OnInventoryOpened.Invoke(this, BoolEventArgs.False);
            OnWeaponsSwitchedToFirstOption.Invoke(this, BoolEventArgs.True);
        }

        public void SetUnitSize(float value)
        {
            _unitSize = value;
            InventoryItem.Init(inventoryCanvas, (float)_unitSize);
        }

        public void SetUpContainers(ContainerBase[] allContainers, ContainerBase mainStorage)
        {
            _allContainers = allContainers;
            _mainStorage = mainStorage;
        }

        public void PickUpItem(InventoryItemData itemData)
        {
            var item = InventoryItemFactory.GetInventoryItem(itemData, (float)_unitSize);
            item.EnableInventoryViewOfItem();
            OnItemPickedUp.Invoke(this, new InventoryItemEventArgs(item, null, mousePos.Vector2));
        }

        private void Update() {
            
            if(openClose.Pressed)
                HandleOpeningClosing();

            if(switchWeapons.Pressed)
                HandleWeaponsSwitching();

            if(mousePos.Vector2 != _lastCursorPos)
                HandleCursorMovement();
            
            if(_allContainers == null || _mainStorage == null) return;

            bool overlapsContainer = false;

            foreach(var c in _allContainers)
            {
                var proj = c.GetProjection(_dragger.DraggedItem, Input.mousePosition);
                if(proj != Projection.EmptyProjection)
                {
                    if(proj != Projection.SameProjection)
                    {
                        _currContainer = c;
                        _proj = proj;
                        OnProjectionChanged.Invoke(this, new ProjectionEventArgs(_proj));
                    }
                    overlapsContainer = true;
                    break;
                }
            }
            if(!overlapsContainer && _currContainer != null)
            {
                OnProjectionChanged.Invoke(this, ProjectionEventArgs.EmptyProjection);
                _currContainer = null;
            }

            _dragger.UpdateDraggerCursor(mousePos.Vector2);

            if(leftClick.Pressed && _currContainer != null)
                if(_dragger.Empty)
                    _dragger.PickUp(_currContainer.ExtractItem(_proj.Replacement));
                else if(_proj.CanPlace)
                    if(TryHandleRefugees())
                        _dragger.PickUp(_currContainer.PlaceItem(_dragger.ExtractItem()));
        }

        private void HandleCursorMovement()
            => OnCursorMoved.Invoke(this, new InventoryItemEventArgs(null, null, _lastCursorPos = mousePos.Vector2));

        private void HandleWeaponsSwitching()
            => OnWeaponsSwitchedToFirstOption.Invoke(this, BoolEventArgs.GetArgs(WeaponsSwitchedToFirstOpt = !WeaponsSwitchedToFirstOpt));

        private void HandleOpeningClosing()
            => OnInventoryOpened.Invoke(this, BoolEventArgs.GetArgs(Opened = !Opened));

        private bool TryHandleRefugees()
        {
            foreach(var i in _proj.Refugees)
                if(!_mainStorage.TryPlaceItemAuto(i))
                {
                    foreach(var j in _proj.Refugees)
                        _mainStorage.ExtractItem(j);
                    return false;                    
                }
            return true;
        }
    }
}

