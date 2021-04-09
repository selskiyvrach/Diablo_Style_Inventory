using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace D2Inventory
{

    public class IconsDrawer 
    {

        private Dictionary<int, Image> _icons = new Dictionary<int, Image>();

        private ObjectPool<Image> _iconsPool;

        private Transform _iconsParent;

        public IconsDrawer(Transform iconsParent)
        {
            var poolSample = new GameObject("Icon").AddComponent<Image>();
            _iconsPool = new ObjectPool<Image>(poolSample, 15, "Icons Pool");
            GameObject.Destroy(poolSample);

            _iconsParent = iconsParent;
        }

        public int CreateIcon(IconInfo info, int iD)
        {
            var icon = _iconsPool.Pop();

            icon.sprite = info.Sprite;
            icon.color = info.Color;
            icon.transform.SetParent(_iconsParent);
            icon.rectTransform.sizeDelta = info.ScreenSize;
            icon.transform.SetAsFirstSibling();
            icon.transform.position = info.ScreenPos;

            _icons.Add(iD, icon);

            return iD;
        }

        public void RemoveIcon(int iD)
        {
            if(_icons.TryGetValue(iD, out Image image))
            {
                _icons.Remove(iD);
                _iconsPool.ReturnItem(image);
            }
        }

        public void MoveIcon(int iD, Vector2 screenPos)
        {
            if(_icons.TryGetValue(iD, out Image image))
            {
                image.transform.SetAsLastSibling();
                image.transform.position = screenPos;
            }
        }

        public void SetIconVisible(int iD, bool value)
        {
            if(_icons.TryGetValue(iD, out Image image))
                image.gameObject.SetActive(value);
        }

    }
    
}
