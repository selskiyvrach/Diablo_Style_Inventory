using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MNS.Utils.Values;
using System;

namespace D2Inventory
{
    
    public class IconManager : MonoBehaviour
    {

        [SerializeField] ItemManager itemManager;
        [SerializeField] float unitSize = 40;
        [SerializeField] IntHandlerSource itemPickedUp;
        [SerializeField] IntHandlerSource draggedItemChanged;
        [SerializeField] IntHandlerSource itemDroppedToWorld;
        [SerializeField] CommonHandlerSource itemEquipped;
        [SerializeField] BoolHandlerSource inventoryOpenClose;

        private ObjectPool<Image> _imagePool;
        private Dictionary<int, Image> _icons = new Dictionary<int, Image>();
        
        private Image _currentlyBeingMoved;
        private int _itemId = -1;

        private void Awake() {
            var sample = Factory.GetGameObjectOfType<Image>();
            _imagePool = new ObjectPool<Image>(sample, 10);
            Destroy(sample.gameObject);
        }

        private void OnEnable() {
            draggedItemChanged.Value.AddWithInvoke((o, args) => HandleItemChanged(args) );
            inventoryOpenClose.Value.AddWithInvoke((o, args) => SetIconsActive(args));
            itemDroppedToWorld.Value.AddWithInvoke((o, args) => RemoveImageForItem(args));
            itemPickedUp.Value.AddWithInvoke((o, args) => SetImageForItem(args));
        }

        private void OnDisable() {   
            draggedItemChanged.Value.Handler -= (o, args) => HandleItemChanged(args);
            inventoryOpenClose.Value.Handler -= (o, args) => SetIconsActive(args);
            itemDroppedToWorld.Value.Handler -= (o, args) => RemoveImageForItem(args);
            itemPickedUp.Value.Handler -= (o, args) => SetImageForItem(args);
        }

        private void LateUpdate() {
            MoveImageForItem();
        }

        private void SetIconsActive(bool value)
        {
            foreach (var i in _icons)
                i.Value.gameObject.SetActive(value);
        }

        private void HandleItemChanged(int newItemId)
        {
            if(newItemId != -1)
                if(_icons.TryGetValue(newItemId, out Image image))
                    _currentlyBeingMoved = image;

            _itemId = newItemId;
        }

        private void SetImageForItem(int iD)
        {
            if(iD == -1) return;

            var image = _imagePool.Pop();
            image.sprite = itemManager.ItemData.ItemInfo[iD].Sprite;
            image.transform.position = itemManager.ItemData.ScreenPos[iD];
            
            Vector2 spriteSize = image.sprite.rect.size;
            Vector2Int sizeInt = itemManager.ItemData.ItemInfo[iD].SizeInt;
            float scale;

            // comparing tecture and sizeInt aspect ratios to decide along which side to scale 
            if(spriteSize.x / spriteSize.y >= (float)sizeInt.x / (float)sizeInt.y)
                scale = (unitSize * sizeInt.x) / spriteSize.x;
            else
                scale = (unitSize * sizeInt.y) / spriteSize.y;

            image.rectTransform.sizeDelta = spriteSize * scale;
            image.transform.position = itemManager.ItemData.cursorPos;
            image.transform.SetParent(transform);
            image.gameObject.SetActive(itemManager.ItemData.VisibleOnScreen[iD]);
            _icons.Add(iD, image);
        }

        private void SetImageForEquippedItem(EventArgs args)
        {
            var itemArgs = args as InventoryItemEventArgs;
            if(itemArgs == null) return;

            _icons[0].transform.position = itemArgs.ScreenPos;
        }

        private void RemoveImageForItem(int iD)
        {
            if(_icons.TryGetValue(iD, out Image value))
            {
                var poolItem = value.GetComponent<PoolItem>();
                if(poolItem != null)
                    poolItem.ReturnToPool();
                else 
                    Destroy(value);
            }
            _icons.Remove(iD);
        }

        private void MoveImageForItem()
        {
            // TODO: figure out the reason for latency in following cursor
            if(_itemId != -1)
                _currentlyBeingMoved.transform.position = itemManager.ItemData.cursorPos;
        }

        private void DeleteImageForItem(int iD)
        {
            if(_icons.TryGetValue(iD, out Image image))
            {
                image.GetComponent<PoolItem>().ReturnToPool();
                _icons.Remove(iD);
            }
        }
    }
}

