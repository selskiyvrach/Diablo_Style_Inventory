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
        // this one is required to put replaced items to when placing to paired containers. See CheckForAction method.
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
            _iconDrawer = new IconDrawer();
// setting default start values
            _onInventoryOpened.Invoke(this, _isOpen = true);
            _onWeaponsSwitchedToFirstOption.Invoke(this, true);
            // _onCursorItemChanged.Invoke(this, null);
            _onProjectionChanged.Invoke(this, Projection.EmptyProjection);

            // TODO: create inputManager
            cursorPos.Getter = () => Input.mousePosition;
            interactButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.Mouse0);
            openCloseButtonPressed.Getter = () => Input.GetKeyDown(KeyCode.I);
            switchWeaponsButonPressed.Getter = () => Input.GetKeyDown(KeyCode.W);
        }

        private void Start() 
            => _unitSize = unitSize.Value.LastArgs;

        private void Update()
        {
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

        public void SwitchWeapons()
            => _onWeaponsSwitchedToFirstOption.Invoke(this, !_onWeaponsSwitchedToFirstOption.LastArgs);

        public void HandleSwitchedWeapons(ContainerBase[] active, ContainerBase[] inactive)
        {            
            foreach(var n in inactive)
                foreach(var m in n.GetContent())
                    if(m != null) 
                    {
                        _onItemUnequipped.Invoke(this, m);
                        foreach(var iconID in m.IconIDs)
                            _iconDrawer.ApplyIconChange(iconID, IconInfo.Hide);
                    }
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
        
        private void MoveCursorItem()
        {
            if (_cursorItem == null) return;

            _cursorItem.DesiredScreenPos = cursorPos.Value;
            _iconDrawer.ApplyIconChange(_cursorItem.IconIDs[0], IconInfo.GetMoveOnly(_cursorItem.DesiredScreenPos));
        }

        public void OpenCloseInventory()
        {
            _onInventoryOpened.Invoke(this, _isOpen = !_isOpen);
            itemIconsParent.gameObject.SetActive(_isOpen);
            backgroundRect.SetActive(_isOpen);
            if(!_isOpen)
                DropCurrentlyCarriedItem(); 
        }
        
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

        private void PerformAction()
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
                        _onItemEquipped.Invoke(this, _cursorItem);
                    }
                    else 
                        Debug.LogError("Couldn't place refugees. Check InventoryController Projection reevaluation on receiving one with refugees");
                }
                else
                    replaced = _lastProjection.Container.ExtractItem(_lastProjection.Replacement);

                if(replaced != null)
                    _onItemUnequipped.Invoke(this, _lastProjection.Replacement);
                _cursorItem = replaced;

                ApplyIconChanges(_lastProjection.Container.GetIconChange());
            }
            else if(_lastProjection.Empty && !backgroundRect.ContainsPoint(cursorPos.Value))
                DropCurrentlyCarriedItem();
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
