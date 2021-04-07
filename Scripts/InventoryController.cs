using System;
using MNS.Events;
using MNS.Utils.Values;
using UnityEngine;


namespace D2Inventory
{

    public class InventoryController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] ChainVector2ValueSource cursorPos;
        [SerializeField] ChainBoolValueSource interactButtonPressed;
        [SerializeField] FloatHandlerSource unitSize;

        public Vector2 CursorPos => cursorPos.Value;

        private EnhancedEventHandler<Projection> _onProjectionChanged = new EnhancedEventHandler<Projection>();
        public IReadOnlyEnhancedHandler<Projection> OnProjectionChanged => _onProjectionChanged;

        private EnhancedEventHandler<bool> _onInventoryOpened = new EnhancedEventHandler<bool>();
        public IReadOnlyEnhancedHandler<bool> OnInventoryOpened => _onInventoryOpened;

        private EnhancedEventHandler<bool> _onWeaponsSwitchedToFirstOption = new EnhancedEventHandler<bool>();
        public IReadOnlyEnhancedHandler<bool> OnWeaponsSwitchedToFirstOption => _onWeaponsSwitchedToFirstOption;
        
        private EnhancedEventHandler<InventoryItem> _onItemPickedUp = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemPickedUp => _onItemPickedUp;

        private EnhancedEventHandler<InventoryItem> _onItemDropped = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemDropped => _onItemDropped;

        private EnhancedEventHandler<Projection> _onItemEquipped = new EnhancedEventHandler<Projection>();
        public IReadOnlyEnhancedHandler<Projection> OnItemEquipped => _onItemEquipped;
        
        private EnhancedEventHandler<InventoryItem> _onItemUnequipped = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemUnequipped => _onItemUnequipped;
                
        private EnhancedEventHandler<InventoryItem> _onCursorItemChanged = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnCursorItemChanged => _onCursorItemChanged;
        
        private EnhancedEventHandler<float> _onUnitSizeChanged = new EnhancedEventHandler<float>();
        public IReadOnlyEnhancedHandler<float> OnUnitSizeChanged => _onUnitSizeChanged;

        private ContainerBase[] _containers;

        private InventoryItem _cursorItem;

        private Projection _lastProjection = Projection.EmptyProjection;

        private void Awake() {
            _onInventoryOpened.Invoke(this, true);
            _onWeaponsSwitchedToFirstOption.Invoke(this, true);
            _onCursorItemChanged.Invoke(this, null);
            _onProjectionChanged.Invoke(this, Projection.EmptyProjection);
            unitSize.Value.AddWithInvoke((o, args) => _onUnitSizeChanged.Invoke(this, args));

            // TODO: create inputManager
            cursorPos.Getter = () => Input.mousePosition;
            interactButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.Mouse0);
        }

        private void OnEnable() {
            unitSize.Value.AddWithInvoke((o, args) => _onUnitSizeChanged.Invoke(this, args));
        }

        private void OnDisable() {
            unitSize.Value.RemoveListener((o, args) => _onUnitSizeChanged.Invoke(this, args));
        }

        private void Update() {
            CheckProjection(cursorPos.Value);
            CheckForAction();
        }

        private void CheckForAction()
        {
            if(interactButtonPressed.Value)
                if(_cursorItem != null)
                    if(_lastProjection.CanPlace)
                    {
                        _onItemEquipped.Invoke(this, _lastProjection);
                        _cursorItem = _lastProjection.Container.PlaceItem(_cursorItem);
                    }
        }

        private void CheckProjection(Vector2 screenPos)
        {
            if(_containers == null || _containers.Length == 0) return;

            bool overlapsAnything = false;
            foreach(var container in _containers)
            {
                var proj = container.GetProjection(_cursorItem, screenPos);
                if(!proj.Empty)
                {   
                    overlapsAnything = true;
                    if(!proj.Same)
                        _onProjectionChanged.Invoke(this, _lastProjection = proj);
                }
            }
            if(!_lastProjection.Empty && !overlapsAnything)
                _onProjectionChanged.Invoke(this, _lastProjection = Projection.EmptyProjection);
        }

        public void SetContainers(ContainerBase[] newContainers)
            => _containers = newContainers;

        public void PickUpItem(InventoryItemData data)
        {
            var item = Factory.GetItem(data);

            if(_cursorItem != null)
                _onItemDropped.Invoke(this, item);

            _onItemPickedUp.Invoke(this, _cursorItem = item);
        }

    }
    
}
