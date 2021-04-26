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
        [Header("Other")]
        [SerializeField] Transform itemIconsParent;
        [SerializeField] ScreenRect backgroundRect;

        private IconDrawer _iconDrawer;

// Trackers
        private bool _isOpen;
        private float _unitSize;
        private ContainerBase[] _containers;
        // The container to put replaced items to. By default it's the big storage in the bottom of inventory, but it can be any container once put at 0 index
        private ContainerBase _mainStorage => _containers != null ? _containers[0] : null;
        private InventoryItem _cursorItem;
        private Projection _lastProjection = Projection.EmptyProjection;
// Behaviour descriptive events
        private EnhancedEventHandler<Projection> _onProjectionChanged = new EnhancedEventHandler<Projection>();
        private EnhancedEventHandler<bool> _onInventoryOpened = new EnhancedEventHandler<bool>();
        private EnhancedEventHandler<bool> _onWeaponsSwitchedToFirstOption = new EnhancedEventHandler<bool>();
        private EnhancedEventHandler<InventoryItem> _onItemPickedUp = new EnhancedEventHandler<InventoryItem>();
        private EnhancedEventHandler<InventoryItem> _onItemDropped = new EnhancedEventHandler<InventoryItem>();
        private EnhancedEventHandler<InventoryItem> _onItemEquipped = new EnhancedEventHandler<InventoryItem>();
        private EnhancedEventHandler<InventoryItem> _onItemUnequipped = new EnhancedEventHandler<InventoryItem>();
// Their public interfaces
        public IReadOnlyEnhancedHandler<Projection> OnProjectionChanged => _onProjectionChanged;
        public IReadOnlyEnhancedHandler<bool> OnInventoryOpened => _onInventoryOpened;
        public IReadOnlyEnhancedHandler<bool> OnWeaponsSwitchedToFirstOption => _onWeaponsSwitchedToFirstOption;
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemPickedUp => _onItemPickedUp;
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemDropped => _onItemDropped;
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemEquipped => _onItemEquipped;
        public IReadOnlyEnhancedHandler<InventoryItem> OnItemUnequipped => _onItemUnequipped;

        private void Awake() {
            // TODO: make IconDrawerWrapper and make it an observer. The problem being initializing items (see PickUpItem(), InitItemIcons())
            _iconDrawer = new IconDrawer();
            // setting default start values
            _onInventoryOpened.Invoke(this, _isOpen = true);
            OpenCloseInventory();
            _onWeaponsSwitchedToFirstOption.Invoke(this, true);
            _onProjectionChanged.Invoke(this, Projection.EmptyProjection);
        }

        private void Start() 
            => unitSize.Value.AddWithInvoke((o, i) => _unitSize = i);

        private void Update()
        {
            
            if(Input.GetKeyDown(KeyCode.O))
            Debug.Log(cursorPos.Value);
            if(openCloseButtonPressed.Value)
                OpenCloseInventory();
            if(switchWeaponsButonPressed.Value)
                SwitchWeapons(); 

            CheckProjection(cursorPos.Value);
            if(_isOpen)
            {
                MoveCursorItem();
                if(interactButtonPressed.Value) 
                    PerformAction();
            }
        }

// Open/Close

        public void OpenCloseInventory()
        {
            _onInventoryOpened.Invoke(this, _isOpen = !_isOpen);
            itemIconsParent.gameObject.SetActive(_isOpen);
            backgroundRect.SetActive(_isOpen);
            if(!_isOpen)
                DropCurrentlyCarriedItem(); 
        }

// Swithing weapons

        public void SwitchWeapons()
            => _onWeaponsSwitchedToFirstOption.Invoke(this, !_onWeaponsSwitchedToFirstOption.LastArgs);

        public void HandleSwitchedWeapons(ContainerBase[] active, ContainerBase[] inactive)
        {            
            // Unequip hidden weapons and hide their icons
            foreach(var n in inactive)
                foreach(var m in n.GetContent())
                    if(m != null) 
                    {
                        _onItemUnequipped.Invoke(this, m);
                        foreach(var iconID in m.IconIDs)
                            _iconDrawer.ApplyIconChange(iconID, IconInfo.Hide);
                    }
            // Equip currently active weapons and reveal their icons
            foreach(var i in active)
                foreach(var j in i.GetContent())
                    if(j != null) 
                    {
                        _onItemEquipped.Invoke(this, j);
                        if(i.ActiveInInventory)
                            foreach(var iconID in j.IconIDs)
                                _iconDrawer.ApplyIconChange(iconID, IconInfo.Reveal);
                    }
        }

// Projection
        
        private void CheckProjection(Vector2 screenPos)
        {
            bool overlapsAnything = false;

            if(_isOpen && _containers != null || _containers.Length > 0) 
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

// Cursor

        private void MoveCursorItem()
        {
            if (_cursorItem == null) return;

            _cursorItem.DesiredScreenPos = cursorPos.Value;
            _iconDrawer.ApplyIconChange(_cursorItem.IconIDs[0], IconInfo.GetMoveOnly(_cursorItem.DesiredScreenPos));
        }

// Interaction

        private void PerformAction()
        {
            if(_lastProjection.CanPlace)
                DoProjectionRelatedAction();
            else if(_lastProjection.Empty && !backgroundRect.ContainsPoint(cursorPos.Value))
                DropCurrentlyCarriedItem();
        }

        private void DoProjectionRelatedAction()
        {
            _lastProjection.Container.RefreshIconCnange();
            _mainStorage.RefreshIconCnange();
            InventoryItem replaced = null;
            if(_cursorItem != null)
                replaced = PlaceItem(replaced);
            else
                replaced = _lastProjection.Container.ExtractItem(_lastProjection.Replacement);

            if (replaced != null)
                _onItemUnequipped.Invoke(this, _lastProjection.Replacement);
            _cursorItem = replaced;

            ApplyIconChanges(_lastProjection.Container.GetIconChange());
        }

        private InventoryItem PlaceItem(InventoryItem replaced)
        {
            if (TryHandleRefugees())
            {
                // applies any changes from handling refugees
                ApplyIconChanges(_mainStorage.GetIconChange());
                replaced = _lastProjection.Container.PlaceItem(_cursorItem);
                _onItemEquipped.Invoke(this, _cursorItem);
            }
            else
                Debug.LogError("Couldn't place refugees. Check InventoryController Projection reevaluation on receiving one with refugees");
            return replaced;
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

// Picking up items

        public void PickUpItem(InventoryItemData data)
        {
            DropCurrentlyCarriedItem();

            _cursorItem = Factory.GetItem(data);
            InitItemIcons();
            _onItemPickedUp.Invoke(this, _cursorItem);

            if (!_onInventoryOpened.LastArgs)
                AutoEquip();
        }

        private void InitItemIcons()
        {
            for (int i = 0; i < _cursorItem.IconIDs.Length; i++)
                _cursorItem.IconIDs[i] = _iconDrawer.CreateIcon(IconInfo.GetAllFieldsUpdated(
                    _cursorItem.ItemData.Sprite, GetItemScreenRect(_cursorItem.ItemData), cursorPos.Value, itemIconsParent, i == 0 ? false : true));
        }

        private void AutoEquip()
        {
            for(int i = _containers.Length - 1; i >= 0; i--)
            {
                var c = _containers[i];
                if(!c.ActiveInInventory) continue;

                c.RefreshIconCnange();
                if(c.TryPlaceItemAuto(_cursorItem))
                {
                    _onItemEquipped.Invoke(this, _cursorItem);
                    ApplyIconChanges(c.GetIconChange());
                    _cursorItem = null;
                    return;
                }
            }
            DropCurrentlyCarriedItem();
        }

// Misc

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
            if(spriteSize.x / spriteSize.y >= (float)data.SizeInt.x / (float)data.SizeInt.y)
                scale = (_unitSize * data.SizeInt.x) / spriteSize.x;
            else
                scale = (_unitSize * data.SizeInt.y) / spriteSize.y;

            return spriteSize * scale * data.ImageScale;
        }
    
        public void SetContainers(ContainerBase[] newContainers)
            => _containers = newContainers;
    }
}