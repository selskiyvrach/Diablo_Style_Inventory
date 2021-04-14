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
        [SerializeField] ChainBoolValueSource openCloseButtonPressed;
        [SerializeField] ChainBoolValueSource switchWeaponsButonPressed;
        [SerializeField] FloatHandlerSource unitSize;
        [SerializeField] Transform itemIconsParent;

        private IconDrawer _iconDrawer;

        private bool _isOpen;

        public IReadOnlyEnhancedHandler<Projection> OnProjectionChanged => _onProjectionChanged;
        private EnhancedEventHandler<Projection> _onProjectionChanged = new EnhancedEventHandler<Projection>();

        public IReadOnlyEnhancedHandler<bool> OnInventoryOpened => _onInventoryOpened;
        private EnhancedEventHandler<bool> _onInventoryOpened = new EnhancedEventHandler<bool>();

        public IReadOnlyEnhancedHandler<bool> OnWeaponsSwitchedToFirstOption => _onWeaponsSwitchedToFirstOption;
        private EnhancedEventHandler<bool> _onWeaponsSwitchedToFirstOption = new EnhancedEventHandler<bool>();
        
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemPickedUp => _onItemPickedUp;
        private EnhancedEventHandler<InventoryItem> _onItemPickedUp = new EnhancedEventHandler<InventoryItem>();

        public IReadOnlyEnhancedHandler<InventoryItem> OnItemDropped => _onItemDropped;
        private EnhancedEventHandler<InventoryItem> _onItemDropped = new EnhancedEventHandler<InventoryItem>();

        public IReadOnlyEnhancedHandler<InventoryItem> OnItemEquipped => _onItemEquipped;
        private EnhancedEventHandler<InventoryItem> _onItemEquipped = new EnhancedEventHandler<InventoryItem>();
        
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemUnequipped => _onItemUnequipped;
        private EnhancedEventHandler<InventoryItem> _onItemUnequipped = new EnhancedEventHandler<InventoryItem>();
                
        public IReadOnlyEnhancedHandler<InventoryItem> OnCursorItemChanged => _onCursorItemChanged;
        private EnhancedEventHandler<InventoryItem> _onCursorItemChanged = new EnhancedEventHandler<InventoryItem>();
        
        public IReadOnlyEnhancedHandler<float> OnUnitSizeChanged => _onUnitSizeChanged;
        private EnhancedEventHandler<float> _onUnitSizeChanged = new EnhancedEventHandler<float>();

        private ContainerBase[] _containers;

        // this one is required to put replaced items to when placing to paired containers. See CheckForAction method.
        // can be used implicitly, but added as a variable for clarity
        private ContainerBase _mainStorage => _containers != null ? _containers[0] : null;

        private InventoryItem _cursorItem;

        private float _unitSize;

        private Projection _lastProjection = Projection.EmptyProjection;

        private void Awake() {
            _iconDrawer = new IconDrawer();

            _onInventoryOpened.Invoke(this, true);
            _onWeaponsSwitchedToFirstOption.Invoke(this, true);
            _onCursorItemChanged.Invoke(this, null);
            _onProjectionChanged.Invoke(this, Projection.EmptyProjection);

            // TODO: create inputManager
            cursorPos.Getter = () => Input.mousePosition;
            interactButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.Mouse0);
            openCloseButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.I);
            switchWeaponsButonPressed.Getter = () => Input.GetKeyDown(KeyCode.W);
        }

        private void OnEnable() {
            unitSize.Value.AddWithInvoke((o, args) => { _onUnitSizeChanged.Invoke(this, args); _unitSize = args; });
        }

        private void OnDisable() {
            unitSize.Value.RemoveListener((o, args) => { _onUnitSizeChanged.Invoke(this, args); _unitSize = args; });
        }

        private void Update()
        {
            CheckOpenClose();
            MoveCursorItem();
            CheckWeaponsSwitch();
            CheckProjection(cursorPos.Value);
            CheckAction();
        }

        private void CheckWeaponsSwitch()
        {
            if(switchWeaponsButonPressed.Value)
                _onWeaponsSwitchedToFirstOption.Invoke(this, !_onWeaponsSwitchedToFirstOption.LastArgs);
        }

        public void HandleSwitchedWeapons(ContainerBase[] active, ContainerBase[] inactive)
        {            
            foreach(var n in inactive)
            {
                foreach(var m in n.GetContent())
                {
                    if(m != null) 
                    {
                        _onItemUnequipped.Invoke(this, m);
                        // _iconDrawer.HideIcon(m.MainIconID);
                        // if(m.SecondTakenContainer != null)
                        //     _iconDrawer.HideIcon(m.SecondIconID);
                    }
                }
            }
            foreach(var i in active)
            {
                foreach(var j in i.GetContent())
                {
                    if(j != null) 
                    {
                        _onItemEquipped.Invoke(this, j);
                        // _iconDrawer.RevealIcon(j.MainIconID);
                        // if(j.SecondTakenContainer != null)
                        //     _iconDrawer.RevealIcon(j.SecondIconID);
                    }
                }

            }
        }
        
        private void MoveCursorItem()
        {
            if (_cursorItem != null)
            {
                _cursorItem.DesiredScreenPos = cursorPos.Value;
                _iconDrawer.ApplyIconChange(_cursorItem.IconIDs[0], IconInfo.GetMoveOnly(_cursorItem.DesiredScreenPos));
            }
        }

        private void CheckOpenClose()
        {
            if(openCloseButtonPressed.Value)
            {
                _onInventoryOpened.Invoke(this, _isOpen = !_isOpen);
                itemIconsParent.gameObject.SetActive(_isOpen);
                if(!_isOpen)
                    DropCurrentlyCarriedItem(); 
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
                    {
                        if(proj.CanPlace && _mainStorage.CanPlaceItemsAuto(proj.Refugees))
                            _onProjectionChanged.Invoke(this, _lastProjection = proj);
                        else   
                            _onProjectionChanged.Invoke(this, _lastProjection = proj.SetCanPlace(false));
                    }
                }
            }
            if(!_lastProjection.Empty && !overlapsAnything)
                _onProjectionChanged.Invoke(this, _lastProjection = Projection.EmptyProjection);
        }

        private void CheckAction()
        {
            if(interactButtonPressed.Value)
            {
                if(_lastProjection.CanPlace)
                {
                    _lastProjection.Container.RefreshIconCnange();
                    _mainStorage.RefreshIconCnange();
                    InventoryItem replaced = null;
                    if(_cursorItem != null)
                    {
                        if(TryHandleRefugees())
                        {
                            // applies any changes from handling refugees
                            ApplyIconChanges(_mainStorage.GetIconChange());
                            replaced = _lastProjection.Container.PlaceItem(_cursorItem);
                            _onCursorItemChanged.Invoke(this, null);
                            _onItemEquipped.Invoke(this, _cursorItem);
                        }
                        else 
                            Debug.LogError("Couldn't place refugees. Check InventoryController Projection reevaluation on receiving one with refugees");
                    }
                    else
                        replaced = _lastProjection.Container.ExtractItem(_lastProjection.Replacement);

                    if(replaced != null)
                        _onItemUnequipped.Invoke(this, _lastProjection.Replacement);
                    _onCursorItemChanged.Invoke(this, _cursorItem = replaced);

                    ApplyIconChanges(_lastProjection.Container.GetIconChange());
                }
                else if(_lastProjection.Empty)
                    DropCurrentlyCarriedItem();
            }
        }

        private void DropCurrentlyCarriedItem()
        {
            if(_cursorItem != null)
            {
                _onItemDropped.Invoke(this, _cursorItem);
                foreach(var i in _cursorItem.IconIDs)
                    _iconDrawer.ApplyIconChange(i, IconInfo.Delete);
            }
            _cursorItem = null;
        }

        private bool TryHandleRefugees()
        {
            if(_lastProjection.Refugees == null || _lastProjection.Refugees.Length == 0) return true;

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

        public void SetContainers(ContainerBase[] newContainers)
            => _containers = newContainers;

        public void PickUpItem(InventoryItemData data)
        {
            var item = Factory.GetItem(data);
            for(int i = 0; i < item.IconIDs.Length; i++)
            {
                item.IconIDs[i] = 
                    _iconDrawer.CreateIcon(IconInfo.GetAllFieldsUpdated(item.ItemData.Sprite, GetItemScreenRect(item.ItemData), cursorPos.Value, Color.white, itemIconsParent));
                if(i > 0)
                    _iconDrawer.ApplyIconChange(item.IconIDs[i], IconInfo.Hide);
            }

            DropCurrentlyCarriedItem();

            _onItemPickedUp.Invoke(this, _cursorItem = item);

            if(!_onInventoryOpened.LastArgs)
            {
                foreach (var i in _containers)
                {
                    if(i.TryPlaceItemAuto(item))
                    {
                        _onItemEquipped.Invoke(this, item);
                        foreach (var iconChange in i.GetIconChange())
                            _iconDrawer.ApplyIconChange(iconChange.Item1, iconChange.Item2);
                        return;
                    }
                }
            }
        }

        private void ApplyIconChanges((int iD, IconInfo info)[] args)
        {
            foreach (var item in args)
                _iconDrawer.ApplyIconChange(item.iD, item.info);
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
