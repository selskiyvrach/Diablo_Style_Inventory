using System;
using MNS.Events;
using MNS.Utils;
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
        [SerializeField] Transform itemIconsParent;

        private IconsDrawer _iconManager;

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

        private EnhancedEventHandler<InventoryItem> _onItemEquipped = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemEquipped => _onItemEquipped;
        
        private EnhancedEventHandler<InventoryItem> _onItemUnequipped = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemUnequipped => _onItemUnequipped;
                
        private EnhancedEventHandler<InventoryItem> _onCursorItemChanged = new EnhancedEventHandler<InventoryItem>();
        public IReadOnlyEnhancedHandler<InventoryItem> OnCursorItemChanged => _onCursorItemChanged;
        
        private EnhancedEventHandler<float> _onUnitSizeChanged = new EnhancedEventHandler<float>();
        public IReadOnlyEnhancedHandler<float> OnUnitSizeChanged => _onUnitSizeChanged;

        private ContainerBase[] _containers;

        // this one is required to put replaced items to when placing to paired containers. See CheckForAction method.
        // can be used implicitly, but added as a variable for clarity
        private ContainerBase _mainStorage => _containers != null ? _containers[0] : null;

        private InventoryItem _cursorItem;

        private float _unitSize;

        private Projection _lastProjection = Projection.EmptyProjection;

        private void Awake() {
            _iconManager = new IconsDrawer(itemIconsParent);

            _onInventoryOpened.Invoke(this, true);
            _onWeaponsSwitchedToFirstOption.Invoke(this, true);
            _onCursorItemChanged.Invoke(this, null);
            _onProjectionChanged.Invoke(this, Projection.EmptyProjection);

            // TODO: create inputManager
            cursorPos.Getter = () => Input.mousePosition;
            interactButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.Mouse0);
        }

        private void OnEnable() {
            unitSize.Value.AddWithInvoke((o, args) => { _onUnitSizeChanged.Invoke(this, args); _unitSize = args; });
        }

        private void OnDisable() {
            unitSize.Value.RemoveListener((o, args) => { _onUnitSizeChanged.Invoke(this, args); _unitSize = args; });
        }

        private void Update() {
            if(_cursorItem != null)
            {
                _cursorItem.DesiredScreenPos = cursorPos.Value;
                // TODO: create iconManagerWrapper ???
                _iconManager.MoveIcon(_cursorItem.ID, cursorPos.Value);
            }

            CheckProjection(cursorPos.Value);
            CheckForAction();
        }

        private void CheckForAction()
        {
            if(interactButtonPressed.Value)
                if(_lastProjection.CanPlace)
                {
                    InventoryItem replaced = null;
                    if(_cursorItem != null)
                    {
                        if(TryHandleRefugees())
                        {
                            foreach (var item in _lastProjection.Refugees)
                                _iconManager.MoveIcon(item.ID, item.DesiredScreenPos);
                                
                            replaced = _lastProjection.Container.PlaceItem(_cursorItem);
                            _onCursorItemChanged.Invoke(this, null);
                            _onItemEquipped.Invoke(this, _cursorItem);
                            _iconManager.MoveIcon(_cursorItem.ID, _cursorItem.DesiredScreenPos);
                        }
                        else 
                            Debug.LogError("Couldn't place refugees. Check InventoryController Projection reevaluation on receiving one with refugees");
                    }
                    replaced ??= _lastProjection.Container.ExtractItem(_lastProjection.Replacement);

                    if(replaced != null)
                        _onItemUnequipped.Invoke(this, _lastProjection.Replacement);
                        
                    _onCursorItemChanged.Invoke(this, _cursorItem = replaced);
                }
        }

        private bool TryHandleRefugees()
        {
            if(_lastProjection.Refugees == null || _lastProjection.Refugees.Length == 0)
                return true;

            if(_mainStorage.CanPlaceItemsAuto(_lastProjection.Refugees))
            {
                foreach (var item in _lastProjection.Refugees)
                {
                    item.Container.ExtractItem(item);
                    _mainStorage.TryPlaceItemAuto(item);
                    _onItemEquipped.Invoke(this, item);
                }
            }
            return true;
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
                    {
                        if(proj.CanPlace && _mainStorage.CanPlaceItemsAuto(_lastProjection.Refugees))
                            _onProjectionChanged.Invoke(this, _lastProjection = proj);
                        else   
                            _onProjectionChanged.Invoke(this, _lastProjection = proj.SetCanPlace(false));
                    }
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
            item.SetID(SimpleID.GetNewID());

            _iconManager.CreateIcon(new IconInfo(item.ItemData.Sprite, GetItemScreenRect(item.ItemData), new Vector2(), Color.white), item.ID);

            if(_cursorItem != null)
                _onItemDropped.Invoke(this, _cursorItem);

            _onItemPickedUp.Invoke(this, _cursorItem = item);
        }

        private Vector2 GetItemScreenRect(InventoryItemData data)
        {
            Vector2 spriteSize = data.Sprite.rect.size;
            float scale;

            // comparing texture and sizeInt aspect ratios to decide along which side to scale 
            if(spriteSize.x / spriteSize.y >= data.SizeInt.x / data.SizeInt.y)
                scale = (_unitSize * data.SizeInt.y) / spriteSize.y;
            else
                scale = (_unitSize * data.SizeInt.x) / spriteSize.x;
            
            return spriteSize * scale * data.ImageScale;
        }

    }
    
}
