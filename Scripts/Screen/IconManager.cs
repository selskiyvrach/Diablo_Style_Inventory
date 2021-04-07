using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MNS.Utils.Values;

namespace D2Inventory
{
    
    public class IconManager : MonoBehaviour
    {
        [SerializeField] InventoryController controller;
        [SerializeField] Transform parentInHierarchy;
        [SerializeField] bool createSubParent; 

        private ObjectPool<Image> _imagePool;
        private List<IIconed> _icons = new List<IIconed>();
        private Transform _parentForIcons;
        
        // trackers
        private IIconed _currentlyBeingMoved;
        private bool _iconsActive;
        private float _unitSize;

        private void Awake() {
            var sample = Factory.GetGameObjectOfType<Image>();
            _imagePool = new ObjectPool<Image>(sample, 10);
            Destroy(sample.gameObject);

            if(createSubParent)
            {
                _parentForIcons = new GameObject("Icons").transform;
                _parentForIcons.SetParent(parentInHierarchy);
            }
            else 
                _parentForIcons = parentInHierarchy;
        }

        private void OnEnable() {
            // TODO: add rescale for icons if unit size changes
            controller.OnUnitSizeChanged.AddWithInvoke((o, args) => _unitSize = args);
            controller.OnInventoryOpened.AddWithInvoke((o, args) => SetIconsActive(args));
            controller.OnItemPickedUp.AddWithInvoke((o, args) => SetImageForItem(args));
            controller.OnCursorItemChanged.AddWithInvoke((o, args) => HandleCursorItemChanged(args));
            controller.OnItemEquipped.AddWithInvoke((o, args) => AnchorIconForEquipped(args));
        }

        private void OnDisable() {   
            controller.OnUnitSizeChanged.RemoveListener((o, args) => _unitSize = args);
            controller.OnInventoryOpened.RemoveListener((o, args) => SetIconsActive(args));
            controller.OnItemPickedUp.RemoveListener((o, args) => SetImageForItem(args));
            controller.OnCursorItemChanged.RemoveListener((o, args) => HandleCursorItemChanged(args));
            controller.OnItemEquipped.RemoveListener((o, args) => AnchorIconForEquipped(args));
        }

        private void LateUpdate() {
            MoveImageForItem();
        }

        private void SetIconsActive(bool value)
        {
            _iconsActive = value;
            foreach (var i in _icons)
                i.Icon.gameObject.SetActive(_iconsActive ? i.Visible : false);
        }

        private void HandleCursorItemChanged(IIconed newIconed)
        {
            if(newIconed == _currentlyBeingMoved)
                _currentlyBeingMoved = null;
            else 
                _currentlyBeingMoved = newIconed;
        }

        private void SetImageForItem(IIconed iconed)
        {
            var image = _imagePool.Pop();
            image.sprite = iconed.Sprite;
            image.transform.position = controller.CursorPos;
            
            Vector2 spriteSize = image.sprite.rect.size;
            float scale;

            // comparing texture and sizeInt aspect ratios to decide along which side to scale 
            if(spriteSize.x / spriteSize.y >= iconed.SizeInt.x / iconed.SizeInt.y)
                scale = (_unitSize * iconed.SizeInt.y) / spriteSize.y;
            else
                scale = (_unitSize * iconed.SizeInt.x) / spriteSize.x;
            
            image.rectTransform.sizeDelta = spriteSize * scale * iconed.Scale;
            image.transform.position = controller.CursorPos;
            image.transform.SetParent(_parentForIcons);
            image.gameObject.SetActive(_iconsActive);
            iconed.Icon = image;

            if(_iconsActive)                
                _currentlyBeingMoved = iconed;

            _icons.Add(iconed);
        }

        private void AnchorIconForEquipped(Projection proj)
        {
            _currentlyBeingMoved.Icon.transform.position = proj.PotentialPlacedPos;
            _currentlyBeingMoved = null;
        }

        private void RemoveImageForItem(IIconed iconed)
        {
            for(int i = 0; i < _icons.Count; i++)
                if(_icons[i] == iconed)
                    _icons.RemoveAt(i);
        }

        private void MoveImageForItem()
        {
            // TODO: figure out the reason for latency in following cursor
            if(_currentlyBeingMoved != null)
                _currentlyBeingMoved.Icon.transform.position = controller.CursorPos;
        }
    }
}

