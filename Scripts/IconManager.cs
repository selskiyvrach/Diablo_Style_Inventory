using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MNS.Utils.Values;

namespace D2Inventory
{
    
    public class IconManager : MonoBehaviour
    {
        [SerializeField] ChainVector2ValueSource cursorPos;
        [SerializeField] FloatHandlerSource unitSize;
        [SerializeField] ProjectionHandlerSource itemPickedUpFromWorld;
        [SerializeField] ProjectionHandlerSource itemDroppedToWorld;
        [SerializeField] ProjectionHandlerSource cursorItemChanged;
        [SerializeField] ProjectionHandlerSource itemEquipped;
        [SerializeField] BoolHandlerSource inventoryOpenClose;

        private ObjectPool<Image> _imagePool;
        private List<IIconed> _icons = new List<IIconed>();
        
        // trackers
        private IIconed _currentlyBeingMoved;
        private bool _iconsActive;
        private float _unitSize;

        private void Awake() {
            var sample = Factory.GetGameObjectOfType<Image>();
            _imagePool = new ObjectPool<Image>(sample, 10);
            Destroy(sample.gameObject);
        }

        private void OnEnable() {
            inventoryOpenClose.Value.AddWithInvoke((o, args) => SetIconsActive(args));
        }

        private void OnDisable() {   
            inventoryOpenClose.Value.Handler -= (o, args) => SetIconsActive(args);
        }

        private void LateUpdate() {
            MoveImageForItem();
        }

        private void SetIconsActive(bool value)
        {
            foreach (var i in _icons)
                i.Icon.gameObject.SetActive((_iconsActive = value) ? i.Visible : false);
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
            image.transform.position = cursorPos.Value;
            
            Vector2 spriteSize = image.sprite.rect.size;
            float scale;

            // comparing texture and sizeInt aspect ratios to decide along which side to scale 
            if(spriteSize.x / spriteSize.y >= iconed.SizeInt.x / iconed.SizeInt.y)
                scale = (_unitSize * iconed.SizeInt.x) / spriteSize.x;
            else
                scale = (_unitSize * iconed.SizeInt.y) / spriteSize.y;

            image.rectTransform.sizeDelta = spriteSize * scale;
            image.transform.position = cursorPos.Value;
            image.transform.SetParent(transform);
            image.gameObject.SetActive(_iconsActive);
            _icons.Add(iconed);
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
                _currentlyBeingMoved.Icon.transform.position = cursorPos.Value;
        }
    }
}

